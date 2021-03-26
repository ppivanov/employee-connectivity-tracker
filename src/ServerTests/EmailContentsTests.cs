using EctBlazorApp.Server.MailKit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.ServerTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EmailContentsTests
    {
        private const string ANY = @".+";
        private const string TEAM_NAME = "Test Team";
        private const string MEMBER_NAME_ONE = "TT User One";
        private const string MEMBER_NAME_TWO = "TT User Two";
        private const int POINTS = 10;
        private const int MARGIN = 20;

        private EmailContents emailContents;

        [TestInitialize]
        public void Setup()
        {
            emailContents = new()
            {
                TeamName = TEAM_NAME,
                PointsThreshold = POINTS,
                MarginDifference = MARGIN,
                Members = new() 
                {
                    new()
                    {
                        Name = MEMBER_NAME_ONE,
                        PastPoints = new() { 0, 2, 0, 0, 0, 3, 0 },
                        CurrentPoints = new() { 0, 0, 1, 0, 0, 0, 0 },
                    }, 
                    new()
                    {
                        Name = MEMBER_NAME_TWO,
                        PastPoints = new() { 0, 2, 0, 0, 0, 3, 0 },
                        CurrentPoints = new() { 0, 0, 1, 0, 0, 0, 0 },
                    }
                }
            };
        }

        [TestCleanup]
        public void Teardown()
        {
            emailContents = null;
        }

        [TestMethod]
        public void VerifyToString_TeamDetailsAndMembersReturned()
        {
            string pattern = $"{ANY}{TEAM_NAME}{ANY}Current total{ANY}{POINTS} points{ANY}{MARGIN}{ANY}{MEMBER_NAME_ONE}{ANY}{MEMBER_NAME_TWO}{ANY}";
            Regex regex = new(pattern);

            string contents = emailContents.ToString();
            Match regexTest = regex.Match(contents);

            Assert.IsTrue(regexTest.Success);
        }

        [DataTestMethod]
        [DataRow(MEMBER_NAME_ONE)]
        [DataRow(MEMBER_NAME_TWO)]
        public void VerifyToString_MemberDetailsReturned(string memberName)
        {
            var member = emailContents.Members.First(m => m.Name.Equals(memberName));
            string memberPattern = $"{ANY}{memberName}{ANY}0{ANY}2{ANY}0{ANY}0{ANY}0{ANY}3{ANY}0{ANY}Total: 5{ANY}0{ANY}1{ANY}0{ANY}0{ANY}0{ANY}0{ANY}0{ANY}Total: 1{ANY}";
            Regex regex = new(memberPattern);

            string contents = member.ToString();
            Match regexTest = regex.Match(contents);

            Assert.IsTrue(regexTest.Success);
        }
    }
}