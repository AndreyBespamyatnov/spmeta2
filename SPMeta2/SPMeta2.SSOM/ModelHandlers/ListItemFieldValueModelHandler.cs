﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using SPMeta2.Common;
using SPMeta2.Definitions;
using SPMeta2.Utils;

namespace SPMeta2.SSOM.ModelHandlers
{
    public class ListItemFieldValueModelHandler : SSOMModelHandlerBase
    {
        #region properties

        public override Type TargetType
        {
            get { return typeof(ListItemFieldValueDefinition); }
        }

        #endregion

        #region methods

        public override void DeployModel(object modelHost, Definitions.DefinitionBase model)
        {
            var list = modelHost.WithAssertAndCast<SPListItem>("modelHost", value => value.RequireNotNull());
            var fieldValue = model.WithAssertAndCast<ListItemFieldValueDefinition>("model", value => value.RequireNotNull());

            ProcessFieldValue(list, list, fieldValue);
        }

        private void ProcessFieldValue(object modelHost, SPListItem listItem, ListItemFieldValueDefinition fieldValue)
        {
            InvokeOnModelEvent(this, new ModelEventArgs
            {
                CurrentModelNode = null,
                Model = null,
                EventType = ModelEventType.OnProvisioning,
                Object = listItem,
                ObjectType = typeof(SPListItem),
                ObjectDefinition = fieldValue,
                ModelHost = modelHost
            });

            if (fieldValue.FieldId.HasValue)
            {
                listItem[fieldValue.FieldId.Value] = fieldValue.Value;
            }
            else if (!string.IsNullOrEmpty(fieldValue.FieldName))
            {
                listItem[fieldValue.FieldName] = fieldValue.Value;
            }
            else
            {
                // TODO
                throw new ArgumentException("Eitehr FieldId or FieldName must be provided.");
            }

            InvokeOnModelEvent(this, new ModelEventArgs
            {
                CurrentModelNode = null,
                Model = null,
                EventType = ModelEventType.OnProvisioned,
                Object = listItem,
                ObjectType = typeof(SPListItem),
                ObjectDefinition = fieldValue,
                ModelHost = modelHost
            });
        }

        #endregion
    }
}
