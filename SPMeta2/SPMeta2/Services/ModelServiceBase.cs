﻿using System;
using System.Collections.Generic;
using System.Linq;
using SPMeta2.Common;
using SPMeta2.Definitions;
using SPMeta2.Extensions;
using SPMeta2.ModelHandlers;
using SPMeta2.ModelHosts;
using SPMeta2.Models;
using SPMeta2.Events;
using SPMeta2.Utils;
using System.Reflection;

namespace SPMeta2.Services
{
    public abstract class ModelServiceBase
    {
        #region constructors

        public ModelServiceBase()
        {
            ModelHandlers = new Dictionary<Type, ModelHandlerBase>();
        }

        #endregion

        #region static

        public static void RegisterModelHandlers<T>(ModelServiceBase instance)
            where T : ModelHandlerBase
        {
            RegisterModelHandlers<T>(instance, Assembly.GetExecutingAssembly());
        }

        public static void RegisterModelHandlers<T>(ModelServiceBase instance, Assembly asm)
            where T : ModelHandlerBase
        {
            instance.ModelHandlers.Clear();

            var handlerTypes = ReflectionUtils.GetTypesFromAssembly<T>(asm);

            foreach (var handlerType in handlerTypes)
            {
                var handlerInstance = Activator.CreateInstance(handlerType) as T;

                if (handlerInstance != null)
                {
                    if (!instance.ModelHandlers.ContainsKey(handlerInstance.TargetType))
                        instance.ModelHandlers.Add(handlerInstance.TargetType, handlerInstance);
                }
            }
        }

        #endregion

        #region properties

        private readonly List<ModelHandlerBase> ModelHandlerEvents = new List<ModelHandlerBase>();
        private ModelNode _activeModelNode = null;

        public Dictionary<Type, ModelHandlerBase> ModelHandlers { get; set; }

        protected virtual List<ModelWeigh> GetModelWeighs()
        {
            return DefaultModelWegh.Weighs;
        }

        #endregion

        #region events

        public EventHandler<ModelEventArgs> OnModelEvent;

        //public EventHandler<ModelDefinitionEventArgs> OnDeployingModel;
        //public EventHandler<ModelDefinitionEventArgs> OnDeployedModel;

        //public EventHandler<ModelDefinitionEventArgs> OnRetractingModel;
        //public EventHandler<ModelDefinitionEventArgs> OnRetractedModel;

        //protected virtual void InvokeOnDeployingModel(object sender, ModelDefinitionEventArgs args)
        //{
        //    if (OnDeployingModel != null) OnDeployingModel(sender, args);
        //}

        //protected virtual void InvokeOnDeployedModel(object sender, ModelDefinitionEventArgs args)
        //{
        //    if (OnDeployedModel != null) OnDeployedModel(sender, args);
        //}

        public EventHandler<OnModelNodeProcessedEventArgs> OnModelNodeProcessed;
        public EventHandler<OnModelNodeProcessingEventArgs> OnModelNodeProcessing;

        #endregion

        #region public API

        public virtual void DeployModel(ModelHostBase modelHost, ModelNode model)
        {
            EnsureModelHandleEvents();

            ProcessModelDeployment(modelHost, model);
        }

        public virtual void RetractModel(ModelHostBase modelHost, ModelNode model)
        {
            EnsureModelHandleEvents();

            // TMP, just web model is supported yet
            // experimental support yet
            // https://github.com/SubPointSolutions/spmeta2/issues/70

            var modelDefinition = model.Value;

            if (modelDefinition is WebDefinition)
                RetractWeb(modelHost, model);

            if (modelDefinition is SiteDefinition)
                RetractSite(modelHost, model);
        }

        #endregion

        #region methods

        protected void InvokeOnModelNodeProcessed(object sender, OnModelNodeProcessedEventArgs args)
        {
            if (OnModelNodeProcessed != null)
                OnModelNodeProcessed(sender, args);
        }

        protected void InvokeOnModelNodeProcessing(object sender, OnModelNodeProcessingEventArgs args)
        {
            if (OnModelNodeProcessing != null)
                OnModelNodeProcessing(sender, args);
        }

        protected virtual ModelHandlerBase ResolveModelHandler(Type modelType)
        {
            ModelHandlerBase result;

            ModelHandlers.TryGetValue(modelType, out result);

            return result;
        }

        private void EnsureModelHandleEvents()
        {
            foreach (var modelHandler in ModelHandlers.Values
                                                      .Where(modelHandler => !ModelHandlerEvents.Contains(modelHandler)))
            {
                modelHandler.OnModelEvent += (s, e) =>
                {
                    if (_activeModelNode != null)
                    {
                        _activeModelNode.InvokeOnModelEvents(e.Object, e.EventType);
                        _activeModelNode.InvokeOnModelContextEvents(s, e);
                    }
                };

                ModelHandlerEvents.Add(modelHandler);
            }
        }

        private void ProcessModelDeployment(object modelHost, ModelNode modelNode)
        {
            InvokeOnModelNodeProcessing(this, new OnModelNodeProcessingEventArgs
            {
                ModelNode = modelNode,
                ModelHost = modelHost
            });

            var modelDefinition = modelNode.Value as DefinitionBase;
            var modelHandler = ResolveModelHandler(modelDefinition.GetType());

            if (modelHandler == null)
            {
                throw new ArgumentNullException(
                    string.Format("Can't find model handler for type:[{0}]. Current ModelService type: [{1}].",
                        modelDefinition.GetType(),
                        GetType()));
            }

            // modelHandler might change a model host to allow continiuation
            // spsite -> spweb -> spcontentype -> spcontentypelink

            // process current model
            if (modelDefinition.RequireSelfProcessing)
            {
                _activeModelNode = modelNode;

                modelNode.State = ModelNodeState.Processing;
                modelHandler.DeployModel(modelHost, modelDefinition);
                modelNode.State = ModelNodeState.Processed;

                _activeModelNode = null;
            }

            InvokeOnModelNodeProcessed(this, new OnModelNodeProcessedEventArgs
            {
                ModelNode = modelNode,
                ModelHost = modelHost
            });

            // TODO
            // here must be a "context change" to provide an ability to continue the
            // deployment chain with changes from SPWeb to SPList

            // sort out child models by types
            var childModelTypes = modelNode.ChildModels
                                       .Select(m => m.Value.GetType())
                                       .GroupBy(t => t);

            foreach (var childModelType in childModelTypes)
            {
                modelHandler.WithResolvingModelHost(modelHost, modelDefinition, childModelType.Key, childModelHost =>
                {
                    var childModels =
                        modelNode.GetChildModels(childModelType.Key);

                    foreach (var childModel in childModels)
                        ProcessModelDeployment(childModelHost, childModel);
                });
            }
        }

        private void RetractSite(object modelHost, ModelNode model)
        {
            var siteDefinition = model.Value as SiteDefinition;
            var modelHandler = ResolveModelHandler(siteDefinition.GetType());
        }

        private void RetractWeb(object modelHost, ModelNode model)
        {
            var webDefinition = model.Value as WebDefinition;
            var modelHandler = ResolveModelHandler(webDefinition.GetType());

            if (modelHandler == null && webDefinition.RequireSelfProcessing)
                throw new ArgumentNullException(string.Format("Can't find model handler for type:[{0}] ", webDefinition.GetType()));

            modelHandler.RetractModel(modelHost, webDefinition);
        }

        #endregion
    }


}
