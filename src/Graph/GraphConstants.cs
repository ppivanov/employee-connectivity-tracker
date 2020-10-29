// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

// Source: https://github.com/microsoftgraph/msgraph-training-aspnet-core/blob/master/demo/GraphTutorial/

// <GraphConstantsSnippet>
namespace EctWebApp
{
    public static class GraphConstants
    {
        // Defines the permission scopes used by the app
        public readonly static string[] Scopes =
        {
            "User.Read",
            "Mail.Read",
            "MailboxSettings.Read",
            "Calendars.Read"
        };
    }
}
// </GraphConstantsSnippet>
