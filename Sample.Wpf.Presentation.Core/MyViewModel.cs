﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Expression.Interactivity.Core;
using Presentation.Core;

namespace Sample.Wpf.Presentation.Core
{
    [MetadataType(typeof(MyViewModelMeta))]
    public class MyViewModel : ViewModel
    {
        public MyViewModel() :
            base(new SimpleBackingStore(), new DataErrorInfo())
        {
            // we can assign a view model validator to the whole view model
            // and/or we can add validation rules to the Rules object
            Validation = new ViewModelValidation<MyViewModel>(this);

            Rules = new Rules();
            Rules.Add(new PropertyChainRule(new []{ this.NameOf(x => x.FullName) }), 
                this.NameOf(x => x.FirstName));
            Rules.Add(new PropertyChainRule(new[] { this.NameOf(x => x.FullName) }),
                this.NameOf(x => x.LastName));

            var validateFullName = new ValidationRule<MyViewModel>(vm => vm.FullName.Length > 3, "Full name must be > 3", this.NameOf(x => x.FullName));
            Rules.Add(validateFullName, this.NameOf(x => x.FirstName));
            Rules.Add(validateFullName, this.NameOf(x => x.LastName));

            ValidateCommand = new AsyncCommand(() => Task.Run(() =>
            {
                // simulate a delay if validating
                Validate().Wait();
                Thread.Sleep(5000);
            }));
        }

        public string FirstName
        {
#if !NET4
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
#else
            get { return GetProperty<string>(this.NameOf(x => x.FirstName)); }
            set { SetProperty(value, this.NameOf(x => x.FirstName)); }
#endif
        }

        public string LastName
        {
#if !NET4
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
#else
            get { return GetProperty<string>(this.NameOf(x => x.LastName)); }
            set { SetProperty(value, this.NameOf(x => x.LastName)); }
#endif
        }

        public string FullName => $"{FirstName} {LastName}";

        public ICommand ValidateCommand { get; private set; }
    }
}
