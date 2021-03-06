using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using PutridParrot.Presentation;
using PutridParrot.Presentation.Attributes;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class CreateInstanceAttributeTests
    {
        #region ViewModels
        interface IMyViewModel
        {
            int Numeric { get; set; }
            string[] Array { get; set; }
            ExtendedObservableCollection<string> Collection { get; }
        }

        class MyViewModel : ViewModel,
            IMyViewModel
        {
            [CreateInstance]
            public int Numeric
            {
                get { return GetProperty<int>(); }
                set { SetProperty(value); }
            }

            [CreateInstance]
            public string[] Array
            {
                get { return GetProperty<string[]>(); }
                set { SetProperty(value); }
            }

            [CreateInstance]
            public ExtendedObservableCollection<string> Collection =>
                GetProperty<ExtendedObservableCollection<string>>();
        }

        class MyViewModelWithoutBacking : ViewModelWithoutBacking,
            IMyViewModel
        {
            private int _numeric;
            private string[] _array;
            private ExtendedObservableCollection<string> _collection;

            [CreateInstance]
            public int Numeric
            {
                get { return GetProperty(ref _numeric); }
                set { SetProperty(ref _numeric, value); }
            }

            [CreateInstance]
            public string[] Array
            {
                get { return GetProperty(ref _array); }
                set { SetProperty(ref _array, value); }
            }

            [CreateInstance]
            public ExtendedObservableCollection<string> Collection =>
                GetProperty(ref _collection);
        }

        class MyViewModelWithModel : ViewModelWithModel,
            IMyViewModel
        {
            class Model
            {
                public int Numeric { get; set; }
                public string[] Array { get; set; }
                public ExtendedObservableCollection<string> Collection { get; set; }
            }

            private readonly Model _model = new Model();

            [CreateInstance]
            public int Numeric
            {
                get { return GetProperty(() => _model.Numeric, v => _model.Numeric = v); }
                set { SetProperty(() => _model.Numeric, v => _model.Numeric = v, value); }
            }

            [CreateInstance]
            public string[] Array
            {
                get { return GetProperty(() => _model.Array, v => _model.Array = v); }
                set { SetProperty(() => _model.Array, v => _model.Array = v, value); }
            }

            [CreateInstance]
            public ExtendedObservableCollection<string> Collection =>
                GetProperty(() => _model.Collection, v => _model.Collection = v);
        }
        #endregion

        [TestCase(typeof(MyViewModel))]
        [TestCase(typeof(MyViewModelWithoutBacking))]
        [TestCase(typeof(MyViewModelWithModel))]
        public void PrimitiveTest_ExpectDefaultToBeAssigned(Type viewModelType)
        {
            var vm = (IMyViewModel)Activator.CreateInstance(viewModelType);

            vm.Numeric
                .Should()
                .Be(0);
        }

        [TestCase(typeof(MyViewModel))]
        [TestCase(typeof(MyViewModelWithoutBacking))]
        [TestCase(typeof(MyViewModelWithModel))]
        public void ArrayTest_ExpectNewArray(Type viewModelType)
        {
            var vm = (IMyViewModel)Activator.CreateInstance(viewModelType);

            vm.Array.Length
                .Should()
                .Be(0);
        }

        [TestCase(typeof(MyViewModel))]
        [TestCase(typeof(MyViewModelWithoutBacking))]
        [TestCase(typeof(MyViewModelWithModel))]
        public void CollectionTest_ExpectCollectionToBeCreated(Type viewModelType)
        {
            var vm = (IMyViewModel)Activator.CreateInstance(viewModelType);

            vm.Collection.Count
                .Should()
                .Be(0);
        }
    }
}