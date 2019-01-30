using Peregrine.WPF.ViewModel;
using StaffManager.Model;
using System.Collections.Generic;
using System.ComponentModel;

namespace StaffManager.ViewModel
{
    public abstract class smViewModelBase : perTreeViewItemViewModelBase
    {
        protected smViewModelBase()
        {
            PropertyChanged += (s, e) =>
                {
                    if (!nameof(IsDirty).Equals(e.PropertyName))
                        RaisePropertyChanged(nameof(IsDirty));
                };
        }

        private bool _isDirty;

        public bool IsDirty
        {
            get => _isDirty;
            protected set => Set(nameof(IsDirty), ref _isDirty, value);
        }

        public void Clean()
        {
            IsDirty = false;
        }

        public bool IsReadOnly => !IsEditing;

        private bool _isEditing;

        public bool IsEditing
        {
            protected get => _isEditing;
            set
            {
                Set(nameof(IsEditing), ref _isEditing, value);
                RaisePropertyChanged(nameof(IsReadOnly));
            }
        }

        public abstract object GetModel();

        public abstract void OnModelSet();

        private string _operationDescription;

        public string OperationDescription
        {
            get => _operationDescription;
            set => Set(nameof(OperationDescription), ref _operationDescription, value);
        }
    }

    // ================================================================================

    public abstract class smViewModelBase<TModel> : smViewModelBase where TModel : smModelBase
    {
        private readonly Dictionary<string, HashSet<string>> _relatedProperties = new Dictionary<string, HashSet<string>>();

        private TModel _model;

        public TModel Model
        {
            get => _model;
            set
            {
                var oldModel = Model;

                if (Set(nameof(Model), ref _model, value))
                {
                    if (oldModel != null)
                        oldModel.PropertyChanged -= ModelPropertyChanged;

                    if (_model != null)
                        _model.PropertyChanged += ModelPropertyChanged;

                    // refresh any VM properties that depend on the model
                    RaisePropertyChanged(string.Empty);

                    OnModelSet();
                }
            }
        }

        public override object GetModel()
        {
            return Model;
        }

        public override bool IsValid => base.IsValid && (Model == null || Model.IsValid);

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (nameof(smModelBase.IsValid).Equals(args.PropertyName))
                RaisePropertyChanged(nameof(IsValid));
            else
                IsDirty = true;

            if (!_relatedProperties.TryGetValue(args.PropertyName, out var vmPropertyNames))
                return;

            foreach (var vmPropertyName in vmPropertyNames)
                RaisePropertyChanged(vmPropertyName);
        }

        /// <summary>
        /// Call RaisePropertyChanged() for a dependent (calculated) property on the ViewModel, whenever the specified Model property is updated  
        /// </summary>
        /// <param name="modelPropertyName"></param>
        /// <param name="vmPropertyName"></param>
        protected void AddModelPropertyDependency(string modelPropertyName, string vmPropertyName)
        {
            if (!_relatedProperties.ContainsKey(modelPropertyName))
                _relatedProperties[modelPropertyName] = new HashSet<string>();

            _relatedProperties[modelPropertyName].Add(vmPropertyName);
        }
    }
}