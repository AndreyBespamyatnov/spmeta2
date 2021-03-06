﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using SPMeta2.Common;
using SPMeta2.Definitions;
using SPMeta2.ModelHandlers;
using SPMeta2.SSOM.ModelHosts;
using SPMeta2.Utils;

namespace SPMeta2.SSOM.ModelHandlers
{
    public class UserCustomActionModelHandler : SSOMModelHandlerBase
    {
        #region properties

        public override Type TargetType
        {
            get { return typeof(UserCustomActionDefinition); }
        }

        #endregion

        #region methods

        public override void DeployModel(object modelHost, DefinitionBase model)
        {
            if (!IsValidHostModelHost(modelHost))
                throw new Exception(string.Format("modelHost of type {0} is not supported.", modelHost.GetType()));

            var customAction = model.WithAssertAndCast<UserCustomActionDefinition>("model", value => value.RequireNotNull());

            var siteModelHost = modelHost.WithAssertAndCast<SiteModelHost>("modelHost", value => value.RequireNotNull());
            var site = siteModelHost.HostSite;

            DeploySiteCustomAction(modelHost, site, customAction);
        }

        private void DeploySiteCustomAction(
            object modelHost,
            SPSite site, UserCustomActionDefinition customActionModel)
        {
            var existingAction = site.UserCustomActions.FirstOrDefault(a => a.Name == customActionModel.Name);

            InvokeOnModelEvent(this, new ModelEventArgs
            {
                CurrentModelNode = null,
                Model = null,
                EventType = ModelEventType.OnProvisioning,
                Object = existingAction,
                ObjectType = typeof(SPUserCustomAction),
                ObjectDefinition = customActionModel,
                ModelHost = modelHost
            });

            if (existingAction == null)
                existingAction = site.UserCustomActions.Add();

            MapCustomAction(existingAction, customActionModel);

            InvokeOnModelEvent(this, new ModelEventArgs
            {
                CurrentModelNode = null,
                Model = null,
                EventType = ModelEventType.OnProvisioned,
                Object = existingAction,
                ObjectType = typeof(SPUserCustomAction),
                ObjectDefinition = customActionModel,
                ModelHost = modelHost
            });

            existingAction.Update();
        }

        private void MapCustomAction(SPUserCustomAction existringAction, UserCustomActionDefinition customAction)
        {
            existringAction.Description = customAction.Description;
            existringAction.Group = customAction.Group;
            existringAction.Location = customAction.Location;
            existringAction.Name = customAction.Name;
            existringAction.ScriptBlock = customAction.ScriptBlock;
            existringAction.ScriptSrc = customAction.ScriptSrc;
            existringAction.Title = customAction.Title;
        }

        private bool IsValidHostModelHost(object modelHost)
        {
            return modelHost is SiteModelHost;
        }

        #endregion
    }
}
