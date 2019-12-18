// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using MalayalamDictionary.DbProvider;

namespace MalayalamDictionary.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly IDictionaryLookup engDictionaryProvider;
        public EchoBot(IDictionaryLookup _engDictionaryProvider) :base()
        {
            this.engDictionaryProvider = _engDictionaryProvider;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var word = turnContext.Activity.Text;
            if (!word.Equals("/start", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var results = engDictionaryProvider.GetMeanings(word);
                    await turnContext.SendActivityAsync(CreateActivityWithResult(results, word), cancellationToken);
                }
                catch (WordNotFoundException ex)
                {
                    await turnContext.SendActivityAsync(CreateActivityWithText(ex.Message), cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await turnContext.SendActivityAsync(CreateActivityWithText($"Internal Server error: {ex.Message}{ex.StackTrace}"), cancellationToken);
                }
            }
            else
            {
                var name = turnContext.Activity.From.Name;
                await turnContext.SendActivityAsync(CreateActivityWithText($"Hello {name}!" ), cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(CreateActivityWithText($"Hello and welcome!"), cancellationToken);
                }
            }
        }



        private IActivity CreateActivityWithText(string message)
        {
            var activity = MessageFactory.Text(message);
            return activity;
        }

        private IActivity CreateActivityWithResult(IEnumerable<string> results,string word)
        {
            string reply = $"Meaning of {word}:\n---------\n";
            int i = 1;
            foreach (var result in results)
            {
                reply += $"{i}. {result.Trim()}\t\n\n";
                i++;
            }
            var activity = MessageFactory.Text(reply);
            string speak = @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
              <voice name='Microsoft Server Speech Text to Speech Voice (en-US, JessaRUS)'>" +
              $"{reply}" + "</voice></speak>";
            activity.Speak = speak;
            return activity;
        }
    }
}
