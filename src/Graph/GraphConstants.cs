// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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
