﻿using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SPMeta2.Common;
using SPMeta2.Definitions;
using SPMeta2.ModelHandlers;
using SPMeta2.SSOM.DefaultSyntax;
using SPMeta2.SSOM.ModelHosts;
using SPMeta2.Utils;

namespace SPMeta2.SSOM.ModelHandlers
{
    public class ListModelHandler : SSOMModelHandlerBase
    {
        #region methods

        public override Type TargetType
        {
            get { return typeof(ListDefinition); }
        }

        public override void WithResolvingModelHost(object modelHost, DefinitionBase model, Type childModelType, Action<object> action)
        {
            var webModelHost = modelHost.WithAssertAndCast<WebModelHost>("modelHost", value => value.RequireNotNull());
            var web = webModelHost.HostWeb;

            var listDefinition = model as ListDefinition;

            if (web != null && listDefinition != null)
            {
                var list = web.GetList(SPUtility.ConcatUrls(web.ServerRelativeUrl, listDefinition.GetListUrl()));

                if (childModelType == typeof(ModuleFileDefinition))
                {
                    action(list.RootFolder);
                }
                else if (childModelType == typeof(FolderDefinition))
                {
                    var folderModelHost = new FolderModelHost
                    {
                        CurrentLibrary = list as SPDocumentLibrary,
                        CurrentLibraryFolder = list.RootFolder,

                        CurrentList = (list as SPDocumentLibrary != null) ? null : list,
                        CurrentListItem = null,
                    };

                    action(folderModelHost);
                }
                else
                {
                    action(list);
                }

                list.Update();
            }
            else
            {
                action(modelHost);
            }
        }

        protected override void DeployModelInternal(object modelHost, DefinitionBase model)
        {
            var webModelHost = modelHost.WithAssertAndCast<WebModelHost>("modelHost", value => value.RequireNotNull());
            var web = webModelHost.HostWeb;

            var listModel = model.WithAssertAndCast<ListDefinition>("model", value => value.RequireNotNull());

            // min provision
            var targetList = GetOrCreateList(modelHost, web, listModel);

            targetList.Title = listModel.Title;

            // SPBug, again & again, must not be null
            targetList.Description = listModel.Description = listModel.Description ?? string.Empty;
            targetList.ContentTypesEnabled = listModel.ContentTypesEnabled;

            InvokeOnModelEvent(this, new ModelEventArgs
            {
                CurrentModelNode = null,
                Model = null,
                EventType = ModelEventType.OnProvisioned,
                Object = targetList,
                ObjectType = typeof(SPList),
                ObjectDefinition = listModel,
                ModelHost = modelHost
            });

            targetList.Update();
        }

        private SPList GetOrCreateList(
            object modelHost,
            SPWeb web, ListDefinition listModel)
        {
            var result = GetListByUrl(web, listModel);

            if (result == null)
            {
                var listId = default(Guid);

                // "SPBug", there are two ways to create lists 
                // (1) by TemplateName (2) by TemplateType 
                if (listModel.TemplateType > 0)
                {
                    listId = web.Lists.Add(listModel.Url, listModel.Description,
                        (SPListTemplateType)listModel.TemplateType);
                }
                else if (!string.IsNullOrEmpty(listModel.TemplateName))
                {
                    // TODO, add some validation
                    var listTemplate = web.ListTemplates[listModel.TemplateName];

                    web.Lists.Add(listModel.Url, listModel.Description, listTemplate);
                }
                else
                {
                    throw new ArgumentException("TemplateType or TemplateName must be defined");
                }

                result = web.Lists[listId];

                InvokeOnModelEvent(this, new ModelEventArgs
                {
                    CurrentModelNode = null,
                    Model = null,
                    EventType = ModelEventType.OnProvisioning,
                    Object = result,
                    ObjectType = typeof(SPList),
                    ObjectDefinition = listModel,
                    ModelHost = modelHost
                });
            }
            else
            {
                InvokeOnModelEvent(this, new ModelEventArgs
                {
                    CurrentModelNode = null,
                    Model = null,
                    EventType = ModelEventType.OnProvisioning,
                    Object = result,
                    ObjectType = typeof(SPList),
                    ObjectDefinition = listModel,
                    ModelHost = modelHost
                });
            }

            return result;
        }

        private static SPList GetListByUrl(SPWeb web, ListDefinition listModel)
        {
            SPList result;

            try
            {
                var targetListUrl = SPUrlUtility.CombineUrl(web.Url, listModel.GetListUrl());
                result = web.GetList(targetListUrl);
            }
            catch
            {
                result = null;
            }

            return result;
        }

        #endregion
    }
}
