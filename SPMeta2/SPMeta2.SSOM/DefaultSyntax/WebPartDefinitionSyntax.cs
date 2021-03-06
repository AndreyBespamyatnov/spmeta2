﻿using SPMeta2.Definitions;
using SPMeta2.Models;
using System;
using System.Web.UI.WebControls.WebParts;
namespace SPMeta2.SSOM.DefaultSyntax
{
    public static class WebPartDefinitionSyntax
    {
        #region methods

        #region behavior support

        public static ModelNode OnCreating(this ModelNode model, Action<WebPartDefinition, WebPart> action)
        {
            model.RegisterModelEvent<WebPartDefinition, WebPart>(Common.ModelEventType.OnUpdating, action);

            return model;
        }

        public static ModelNode OnCreated(this ModelNode model, Action<WebPartDefinition, WebPart> action)
        {
            model.RegisterModelEvent<WebPartDefinition, WebPart>(Common.ModelEventType.OnUpdated, action);

            return model;
        }


        #endregion

        #endregion
    }
}
