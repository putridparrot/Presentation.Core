﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Tests.Presentation.Core
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ViewModelWithBackingStoreTests
    {
        [Test]
        public void ChangeProperty_ExpectDirtyFlagSet()
        {
            var vm = new MyViewModel1();

            vm.Name = "Scooby Doo";

            Assert.IsTrue(vm.IsDirty);
        }

        [Test]
        public void ChangeProperty_ToSameValue_ExpectNoChanges()
        {
            var vm = new MyViewModel1();
            var nl = new NotifyPropertyChangedListener(vm);

            vm.BeginInit();
            vm.Name = "Scooby Doo";
            vm.EndInit();

            vm.Name = "Scooby Doo";

            Assert.IsFalse(vm.IsDirty);
            Assert.AreEqual(0, nl.Changing.Count);
            Assert.AreEqual(0, nl.Changed.Count);
        }

        [Test]
        public void ChangeProperty_ExpectPropertyChanges()
        {
            var vm = new MyViewModel1();
            var nl = new NotifyPropertyChangedListener(vm);

            vm.Name = "Scooby Doo";

            // name change + isDirty change expected
            Assert.AreEqual(2, nl.Changing.Count);
            Assert.AreEqual(2, nl.Changed.Count);
        }


        [Test]
        public void BeginInit_ExpectNoPropertyChangeEvents()
        {
            var vm = new MyViewModel1();
            var nl = new NotifyPropertyChangedListener(vm);

            vm.BeginInit();
            vm.Name = "Scooby Doo";
            vm.Address = "Mystery Machine";
            vm.EndInit();

            Assert.AreEqual(0, nl.Changing.Count);
            Assert.AreEqual(0, nl.Changed.Count);
        }

        [Test]
        public void BeginInit_ExpectDirtyFlagToBeUnchanged()
        {
            var vm = new MyViewModel1();

            vm.BeginInit();
            vm.Name = "Scooby Doo";
            vm.Address = "Mystery Machine";
            vm.EndInit();

            Assert.IsFalse(vm.IsDirty);
        }

        [Test]
        public void BeginUpdate_ExpectNoPropertyChangeEvents()
        {
            var vm = new MyViewModel1();
            var nl = new NotifyPropertyChangedListener(vm);

            vm.BeginUpdate();
            vm.Name = "Scooby Doo";
            vm.Address = "Mystery Machine";

            // expect no changes until EndUpdateCalled
            Assert.AreEqual(0, nl.Changing.Count);
            Assert.AreEqual(0, nl.Changed.Count);

            vm.EndUpdate();

            // end up is not subtle so will simply try to refresh all properties
            // no "changing" event occurs
            Assert.AreEqual(0, nl.Changing.Count);
            Assert.AreEqual(1, nl.Changed.Count);
        }

        [Test]
        public void Validate_WithNoValues_ExpectFailure()
        {
            var vm = new MyViewModel1();

            Assert.IsFalse(vm.Validate().Result);
        }

        [Test]
        public void IsBusy_IfChangedShouldResultInPropertyChangeButNotSetIsDirtyToTrue()
        {
            var vm = new MyViewModel1();
            var nl = new NotifyPropertyChangedListener(vm);

            vm.IsBusy = true;

            Assert.AreEqual(1, nl.Changed.Count);
            Assert.AreEqual("IsBusy", nl.Changed[0]);
        }
    }
}
