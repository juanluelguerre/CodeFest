// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace CodeFestBot.Dialogs
{
	public class TwitterDialog : CancelAndHelpDialog
	{
		private const string PromptMsgText = "¿Cuantos tweets quieres?";
		private const string RepromptMsgText = "Esos son muchos tweets y seria muy aburrido tener que leerlos todos !";

		public TwitterDialog()
			: base( nameof(TwitterDialog))
		{

			AddDialog(new TextPrompt(nameof(TextPrompt)));
			AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
			// AddDialog(new DateTimePrompt(nameof(DateTimePrompt), DateTimePromptValidator));
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
			{
				TopNStepAsync
				// FinalStepAsync,
			}));

			// The initial child Dialog to run.
			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> TopNStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var top = (int)stepContext.Options;

			var promptMessage = MessageFactory.Text(PromptMsgText, PromptMsgText, InputHints.ExpectingInput);
			var repromptMessage = MessageFactory.Text(RepromptMsgText, RepromptMsgText, InputHints.ExpectingInput);

			// bool isANumber = int.TryParse(strTop, out int top);
			// if (!isANumber)
			if (top < 1 || top > 5)
			{
				return await stepContext.PromptAsync(nameof(TextPrompt),
					new PromptOptions
					{
						Prompt = promptMessage,
						RetryPrompt = repromptMessage,
					}, cancellationToken);
			}

			// This is essentially a "reprompt" of the data we were given up front.
			//if (top < 1 || top > 5)
			//{
				//return await stepContext.PromptAsync(nameof(TextPrompt),
				//	new PromptOptions
				//	{
				//		Prompt = repromptMessage,
				//	}, cancellationToken);
			// }

			return await stepContext.NextAsync(cancellationToken);
			// return await stepContext.EndDialogAsync(top, cancellationToken);
		}
	}
}
