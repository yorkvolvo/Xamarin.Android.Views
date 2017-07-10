using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Java.Lang;

namespace RecyclerViewSearch
{
    public class RecyclerViewAdapter : RecyclerView.Adapter, IFilterable
    {
        private List<Chemical> _originalData;
        private List<Chemical> _items;
        private readonly Activity _context;

        public Filter Filter { get; private set; }

        public RecyclerViewAdapter(Activity activity, IEnumerable<Chemical> chemicals)
        {
            _items = chemicals.OrderBy(s => s.Name).ToList();
            _context = activity;

            Filter = new ChemicalFilter(this);
        }

        public override long GetItemId(int position)
        {
            return position;
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Chemical, parent, false);
            ChemicalHolder vh = new ChemicalHolder(itemView);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ChemicalHolder vh = holder as ChemicalHolder;

            var chemical = _items[position];

            vh.Image.SetImageResource(chemical.DrawableId);
            vh.Caption.Text = chemical.Name;
        }

        public override int ItemCount
        {
            get { return _items.Count; }
        }

        public class ChemicalHolder : RecyclerView.ViewHolder
        {
            public ImageView Image { get; private set; }
            public TextView Caption { get; private set; }

            public ChemicalHolder(View itemView) : base(itemView)
            {
                Image = itemView.FindViewById<ImageView>(Resource.Id.chemImage);
                Caption = itemView.FindViewById<TextView>(Resource.Id.chemName);
            }
        }

        private class ChemicalFilter : Filter
        {
            private readonly RecyclerViewAdapter _adapter;
            public ChemicalFilter(RecyclerViewAdapter adapter)
            {
                _adapter = adapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                var returnObj = new FilterResults();
                var results = new List<Chemical>();
                if (_adapter._originalData == null)
                    _adapter._originalData = _adapter._items;
                
                if (constraint == null) return returnObj;

                if (_adapter._originalData != null && _adapter._originalData.Any())
                {
                    // Compare constraint to all names lowercased. 
                    // It they are contained they are added to results.
                    results.AddRange(
                        _adapter._originalData.Where(
                            chemical => chemical.Name.ToLower().Contains(constraint.ToString())));
                }
                
                // Nasty piece of .NET to Java wrapping, be careful with this!
                returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
                returnObj.Count = results.Count;

                constraint.Dispose();

                return returnObj;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                using (var values = results.Values)
                    _adapter._items = values.ToArray<Java.Lang.Object>()
                        .Select(r => r.ToNetObject<Chemical>()).ToList();
                    
                _adapter.NotifyDataSetChanged();

                // Don't do this and see GREF counts rising
                constraint.Dispose();
                results.Dispose();
            }
        }
    }

}