using System.Collections.Generic;
using Android.App;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.OS;


namespace RecyclerViewSearch
{
    [Activity(Label = "RecyclerViewSearch", MainLauncher = true, Icon = "@drawable/icon",Theme = "@style/Theme.AppCompat.Light")]
    public class MainActivity : ActionBarActivity
    {

        private Android.Support.V7.Widget.SearchView _searchView;
        private RecyclerView _recyclerView;
        RecyclerView.LayoutManager _LayoutManager;
        private RecyclerViewAdapter _adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            var chemicals = new List<Chemical>
            {
                new Chemical {Name = "Niacin", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Biotin", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Chromichlorid", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Natriumselenit", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Manganosulfate", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Natriummolybdate", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Ergocalciferol", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Cyanocobalamin", DrawableId = Resource.Drawable.Icon},
            };

            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _adapter = new RecyclerViewAdapter(this,chemicals);
            _LayoutManager = new LinearLayoutManager(this);
            _recyclerView.SetLayoutManager(_LayoutManager);
            _recyclerView.SetAdapter(_adapter);//
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main, menu);

            var item = menu.FindItem(Resource.Id.action_search);

            var searchView = MenuItemCompat.GetActionView(item);
            _searchView = searchView.JavaCast<Android.Support.V7.Widget.SearchView>();

            _searchView.QueryTextChange += (s, e) => _adapter.Filter.InvokeFilter(e.NewText);

            _searchView.QueryTextSubmit += (s, e) =>
            {
                // Handle enter/search button on keyboard here
                Toast.MakeText(this, "Searched for: " + e.Query, ToastLength.Short).Show();
                e.Handled = true;
            };

            MenuItemCompat.SetOnActionExpandListener(item, new SearchViewExpandListener(_adapter));

            return true;
        }

        private class SearchViewExpandListener : Java.Lang.Object, MenuItemCompat.IOnActionExpandListener
        {
            private readonly IFilterable _adapter;

            public SearchViewExpandListener(IFilterable adapter)
            {
                _adapter = adapter;
            }

            public bool OnMenuItemActionCollapse(IMenuItem item)
            {
                _adapter.Filter.InvokeFilter("");
                return true;
            }

            public bool OnMenuItemActionExpand(IMenuItem item)
            {
                return true;
            }
        }
    }
}

