﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using VanArsdel.Data;
using VanArsdel.Inventory.Models;
using VanArsdel.Inventory.Providers;
using VanArsdel.Inventory.Views;
using Windows.UI.Xaml.Controls;

namespace VanArsdel.Inventory.ViewModels
{
    public class OrderListViewModel : ListViewModel<OrderModel>
    {
        public OrderListViewModel(IDataProviderFactory providerFactory) : base(providerFactory)
        {
        }

        public OrdersViewState ViewState { get; private set; }

        public async Task LoadAsync(OrdersViewState state)
        {
            ViewState = state ?? OrdersViewState.CreateDefault();
            ApplyViewState(ViewState);
            await base.RefreshAsync();
        }

        public void Unload()
        {
            UpdateViewState(ViewState);
        }

        public override async void New()
        {
            long customerID = ViewState.CustomerID;
            if (IsMainView)
            {
                await ViewManager.Current.CreateNewView(typeof(OrderView), new OrderViewState(customerID));
            }
            else
            {
                NavigationService.Main.Navigate(typeof(OrderView), new OrderViewState(customerID));
            }
        }

        override public async Task<PageResult<OrderModel>> GetItemsAsync(IDataProvider dataProvider)
        {
            var request = new PageRequest<Order>(PageIndex, PageSize)
            {
                Query = Query,
                OrderBy = ViewState.OrderBy,
                OrderByDesc = ViewState.OrderByDesc
            };
            if (ViewState.CustomerID > 0)
            {
                request.Where = (r) => r.CustomerID == ViewState.CustomerID;
            }
            return await dataProvider.GetOrdersAsync(request);
        }

        protected override async Task DeleteItemsAsync(IDataProvider dataProvider, IEnumerable<OrderModel> models)
        {
            foreach (var model in models)
            {
                await dataProvider.DeleteOrderAsync(model);
            }
        }

        protected override async Task<bool> ConfirmDeleteSelectionAsync()
        {
            return await DialogBox.ShowAsync("Confirm Delete", "Are you sure you want to delete selected orders?", "Ok", "Cancel");
        }

        public CustomersViewState GetCurrentState()
        {
            var state = CustomersViewState.CreateDefault();
            UpdateViewState(state);
            return state;
        }
    }
}
