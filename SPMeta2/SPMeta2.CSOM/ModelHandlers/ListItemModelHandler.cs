﻿using Microsoft.SharePoint.Client;
using SPMeta2.Common;
using SPMeta2.Definitions;
using SPMeta2.ModelHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPMeta2.Utils;
using SPMeta2.CSOM.ModelHosts;

namespace SPMeta2.CSOM.ModelHandlers
{
    public class ListItemModelHandler : CSOMModelHandlerBase
    {
        #region properties

        public override Type TargetType
        {
            get { return typeof(ListItemDefinition); }
        }

        #endregion

        #region methods

        public override void DeployModel(object modelHost, Definitions.DefinitionBase model)
        {
            var list = modelHost.WithAssertAndCast<List>("modelHost", value => value.RequireNotNull());
            var listItemModel = model.WithAssertAndCast<ListItemDefinition>("model", value => value.RequireNotNull());

            DeployInternall(list, listItemModel);
        }

        private void DeployInternall(List list, ListItemDefinition listItemModel)
        {
            if (IsDocumentLibray(list))
            {
                throw new NotImplementedException("Please use ModuleFileDefinition to deploy files to the document libraries");
            }

            ListItem currentItem = null;

            InvokeOnModelEvent<ListItemDefinition, ListItem>(currentItem, ModelEventType.OnUpdating);
            currentItem = EnsureListItem(list, listItemModel);
            InvokeOnModelEvent<ListItemDefinition, ListItem>(currentItem, ModelEventType.OnUpdated);
        }

        public override void WithResolvingModelHost(object modelHost, DefinitionBase model, Type childModelType, Action<object> action)
        {
            var list = modelHost.WithAssertAndCast<List>("modelHost", value => value.RequireNotNull());
            var listItemModel = model.WithAssertAndCast<ListItemDefinition>("model", value => value.RequireNotNull());

            var item = EnsureListItem(list, listItemModel);
            var context = list.Context;

            if (childModelType == typeof(ListItemFieldValueDefinition))
            {
                // naaaaah, just gonna get a new one list item
                // keep it simple and safe, really really really safe with all that SharePoint stuff...
                var currentItem = list.GetItemById(item.Id);

                context.Load(currentItem);
                context.ExecuteQuery();

                var listItemPropertyHost = new ListItemFieldValueModelHost
                {
                    CurrentItem = currentItem
                };

                action(listItemPropertyHost);

                currentItem.Update();

                context.ExecuteQuery();
            }
        }

        private ListItem EnsureListItem(List list, ListItemDefinition listItemModel)
        {
            var context = list.Context;

            // TODO, lazy to query
            var items = list.GetItems(CamlQuery.CreateAllItemsQuery());

            context.Load(items);
            context.ExecuteQuery();

            // BIG TODO, don't tell me, I know that
            var currentItem = items.FirstOrDefault(i => i["Title"] != null &&
                    (i["Title"].ToString() == listItemModel.Title));

            InvokeOnModelEvent(this, new ModelEventArgs
            {
                CurrentModelNode = null,
                Model = null,
                EventType = ModelEventType.OnProvisioning,
                Object = currentItem,
                ObjectType = typeof(ListItem),
                ObjectDefinition = listItemModel,
                ModelHost = list
            });

            if (currentItem == null)
            {
                var newItem = list.AddItem(new ListItemCreationInformation());

                newItem["Title"] = listItemModel.Title;

                InvokeOnModelEvent(this, new ModelEventArgs
                {
                    CurrentModelNode = null,
                    Model = null,
                    EventType = ModelEventType.OnProvisioned,
                    Object = newItem,
                    ObjectType = typeof(ListItem),
                    ObjectDefinition = listItemModel,
                    ModelHost = list
                });

                newItem.Update();

                context.ExecuteQuery();

                return newItem;
            }
            else
            {
                currentItem["Title"] = listItemModel.Title;

                InvokeOnModelEvent(this, new ModelEventArgs
                {
                    CurrentModelNode = null,
                    Model = null,
                    EventType = ModelEventType.OnProvisioned,
                    Object = currentItem,
                    ObjectType = typeof(ListItem),
                    ObjectDefinition = listItemModel,
                    ModelHost = list
                });

                currentItem.Update();

                context.ExecuteQuery();

                return currentItem;
            }
        }

        private bool IsDocumentLibray(List list)
        {
            // TODO
            return false;
        }

        #endregion
    }
}
