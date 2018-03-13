﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using VanArsdel.Inventory.Models;
using VanArsdel.Inventory.Providers;

namespace VanArsdel.Inventory.ViewModels
{
    public class OrderItemDetailsViewModel : DetailsViewModel<OrderItemModel>
    {
        public OrderItemDetailsViewModel(IDataProviderFactory providerFactory) : base(providerFactory)
        {
        }

        override public string Title => ((Item?.IsNew) ?? false) ? "New OrderItem" : $"Order #{Item?.OrderID}, {Item?.OrderLine}" ?? String.Empty;

        protected override void ItemUpdated()
        {
            NotifyPropertyChanged(nameof(Title));
        }

        public async Task LoadAsync(OrderItemViewState state)
        {
            if (state.OrderLine > 0)
            {
                using (var dp = ProviderFactory.CreateDataProvider())
                {
                    Item = await dp.GetOrderItemAsync(state.OrderID, state.OrderLine);
                }
            }
            else
            {
                Item = new OrderItemModel();
                IsEditMode = true;
            }
        }

        protected override async Task SaveItemAsync(OrderItemModel model)
        {
            using (var dataProvider = ProviderFactory.CreateDataProvider())
            {
                await dataProvider.UpdateOrderItemAsync(model);
            }
        }

        protected override async Task DeleteItemAsync(OrderItemModel model)
        {
            using (var dataProvider = ProviderFactory.CreateDataProvider())
            {
                await dataProvider.DeleteOrderItemAsync(model);
            }
        }

        override protected IEnumerable<IValidationConstraint<OrderItemModel>> ValidationConstraints
        {
            get
            {
                // TODO: 
                yield break;
            }
        }
    }
}