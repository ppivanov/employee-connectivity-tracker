﻿using EctBlazorApp.Client.Pages.DashboardClasses;
using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.ServerTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class MyTeamDashboardClassTests
    {
        private MyTeamDashboardClass _componentClass;

        [TestInitialize]
        public void BeforeEach() => ResetComponentClassProperties();

        [TestMethod]
        public void AddUserToNotify_BothFieldsEmpty_ErrorMessage()
        {
            _componentClass.AddUserToNotify().Wait();

            Assert.IsTrue(_componentClass.ServerMessageIsError);
            Assert.IsTrue(_componentClass.AddNotifyUserInputError);
            Assert.AreNotEqual("", _componentClass.ServerMessage);
        }

        [TestMethod]
        public void AddUserToNotify_EmailFieldEmpty_ErrorMessage()
        {
            _componentClass.UserToNotify_Name = "John Doe";

            _componentClass.AddUserToNotify().Wait();

            Assert.IsTrue(_componentClass.ServerMessageIsError);
            Assert.IsTrue(_componentClass.AddNotifyUserInputError);
            Assert.AreNotEqual("", _componentClass.ServerMessage);
            Assert.AreNotEqual("", _componentClass.UserToNotify_Name);
        }

        [TestMethod]
        public void AddUserToNotify_NameFieldEmpty_ErrorMessage()
        {
            _componentClass.UserToNotify_Email = "john@ect.ie";

            _componentClass.AddUserToNotify().Wait();

            Assert.IsTrue(_componentClass.ServerMessageIsError);
            Assert.IsTrue(_componentClass.AddNotifyUserInputError);
            Assert.AreNotEqual("", _componentClass.ServerMessage);
            Assert.AreNotEqual("", _componentClass.UserToNotify_Email);
        }

        [TestMethod]
        public void AddUserToNotify_NameAndEmailOk_EntryUserAdded()
        {
            const string name = "John Doe";
            const string email = "john@ect.ie";
            string entryToAdd = FormatFullNameAndEmail(name, email);
            _componentClass = MockJsInterop();
            _componentClass.UserToNotify_Name = name;
            _componentClass.UserToNotify_Email = email;

            _componentClass.AddUserToNotify().Wait();
            bool isUserInList = _componentClass.NewNotificationOptions.UsersToNotify.Contains(entryToAdd);


            Assert.IsTrue(isUserInList);
            Assert.IsFalse(_componentClass.ServerMessageIsError);
            Assert.IsFalse(_componentClass.AddNotifyUserInputError);
            Assert.AreEqual("", _componentClass.ServerMessage);
            Assert.AreEqual("", _componentClass.UserToNotify_Email);
            Assert.AreEqual("", _componentClass.UserToNotify_Name);
        }

        [TestMethod]
        public void AddUserToNotify_NameAndEmailAlreadyInList_EntryNotAdded()
        {
            const string name = "Alice AliceS";
            const string email = "alice@ect.ie";
            string entryToAdd = FormatFullNameAndEmail(name, email);
            _componentClass.UserToNotify_Name = name;
            _componentClass.UserToNotify_Email = email;

            _componentClass.AddUserToNotify().Wait();
            bool isUserInList = _componentClass.NewNotificationOptions.UsersToNotify.Contains(entryToAdd);


            Assert.IsTrue(isUserInList);
            Assert.IsTrue(_componentClass.ServerMessageIsError);
            Assert.IsTrue(_componentClass.AddNotifyUserInputError);
            Assert.AreNotEqual("", _componentClass.ServerMessage);
            Assert.AreEqual(email, _componentClass.UserToNotify_Email);
            Assert.AreEqual(name, _componentClass.UserToNotify_Name);
        }

        [TestMethod]
        public void RemoveUserToNotify_RemoveAlice_AliceIsRemovedFromList()
        {
            const string name = "Alice AliceS";
            const string email = "<alice@ect.ie>";
            string entryToRemove = FormatFullNameAndEmail(name, email);

            _componentClass.RemoveUserToNotify(entryToRemove);
            bool isUserInList = _componentClass.NewNotificationOptions.UsersToNotify.Contains(entryToRemove);

            Assert.IsFalse(isUserInList);
        }

        [TestMethod]
        public void SetUserToNotifyEmail_EnterRandomEmail_NameFieldNotChanged()
        {
            const string email = "random-email@ect.ie";
            ChangeEventArgs args = new() { Value = email };
            _componentClass = MockJsInterop();

            _componentClass.SetUserToNotifyEmail(args).Wait();

            Assert.AreEqual("", _componentClass.UserToNotify_Name);
        }

        [TestMethod]
        public void SetUserToNotifyEmail_EnterMemberEmail_NameFieldChanged()
        {
            const string email = "bob@ect.ie";
            const string expectedName = "Bob BobS";
            ChangeEventArgs args = new() { Value = email };
            _componentClass = MockJsInterop();

            _componentClass.SetUserToNotifyEmail(args).Wait();

            Assert.AreEqual(expectedName, _componentClass.UserToNotify_Name);
        }

        [TestMethod]
        public void SetUserToNotifyName_EnterRandomName_EmailFieldNotChanged()
        {
            const string name = "Random Name";
            ChangeEventArgs args = new() { Value = name };
            _componentClass = MockJsInterop();

            _componentClass.SetUserToNotifyName(args).Wait();

            Assert.AreEqual("", _componentClass.UserToNotify_Email);
        }

        [TestMethod]
        public void SetUserToNotifyName_EnterMemberName_EmailFieldChanged()
        {
            const string name = "Bob BobS";
            const string expectedEmail = "bob@ect.ie";
            ChangeEventArgs args = new() { Value = name };
            _componentClass = MockJsInterop();

            _componentClass.SetUserToNotifyName(args).Wait();

            Assert.AreEqual(expectedEmail, _componentClass.UserToNotify_Email);
        }

        private void ResetComponentClassProperties()
        {
            NotificationOptionsResponse currentNotificationOptions = new()
            {
                PointsThreshold = 10,
                MarginForNotification = 10,
                UsersToNotify = new()
                {
                    "Alice AliceS <alice@ect.ie>"
                }
            };
            NotificationOptionsResponse newNotificationOptions = new(currentNotificationOptions);
            List<EctUser> administrators = new()
            {
                new()
                {
                    Email = "admin@ect.ie",
                    FullName = "Admin AdminS"
                }
            };
            List<EctUser> teamMembers = new()
            {
                new()
                {
                    Email = "alice@ect.ie",
                    FullName = "Alice AliceS"
                },
                new()
                {
                    Email = "bob@ect.ie",
                    FullName = "Bob BobS"
                },
                new()
                {
                    Email = "mike@ect.ie",
                    FullName = "Mike MikeS"
                }
            };

            _componentClass = new(administrators, teamMembers, currentNotificationOptions, newNotificationOptions);
            _componentClass.AddNotifyUserInputError = false;
            _componentClass.ServerMessageIsError = false;
            _componentClass.UserToNotify_Email = "";
            _componentClass.UserToNotify_Name = "";
            _componentClass.ServerMessage = "";
        }

        private MyTeamDashboardClass MockJsInterop()
        {
            Mock<MyTeamDashboardClass> mockInstance = new() { CallBase = true };
            mockInstance.Setup(tdc => tdc.JsInterop(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            mockInstance.Object.Administrators = _componentClass.Administrators;
            mockInstance.Object.NewNotificationOptions = _componentClass.NewNotificationOptions;
            mockInstance.Object.TeamMembers = _componentClass.TeamMembers;

            // Copy over other properties if required

            return mockInstance.Object;
        }
    }
}
