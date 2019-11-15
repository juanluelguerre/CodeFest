//// Copyright (c) Microsoft Corporation. All rights reserved.
//// Licensed under the MIT License.
////
//// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Bot.Builder;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Schema;
//using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

//namespace CodeFestBot.Dialogs
//{
//	public class TopResolverDialog : CancelAndHelpDialog
//	{
//		private const string PromptMsgText = "�Cuantos tweets quieres?";
//		private const string RepromptMsgText = "Esos son muchos tweets y seria muy aburrido tener que leerlos todos !";

//		public TopResolverDialog(string id = null)
//			: base(id ?? nameof(TopResolverDialog))
//		{
//			// AddDialog(new DateTimePrompt(nameof(DateTimePrompt), DateTimePromptValidator));
//			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
//			{
//				InitialStepAsync
//				// FinalStepAsync,
//			}));

//			// The initial child Dialog to run.
//			InitialDialogId = nameof(WaterfallDialog);
//		}

//		private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//		{
//			var strTop = (string)stepContext.Options;

//			var promptMessage = MessageFactory.Text(PromptMsgText, PromptMsgText, InputHints.ExpectingInput);
//			var repromptMessage = MessageFactory.Text(RepromptMsgText, RepromptMsgText, InputHints.ExpectingInput);

//			bool isANumber = int.TryParse(strTop, out int top);

//			if (!isANumber)
//			{				
//				return await stepContext.PromptAsync(nameof(TextPrompt),
//					new PromptOptions
//					{
//						Prompt = promptMessage,
//						RetryPrompt = repromptMessage,
//					}, cancellationToken);
//			}
			
//			// This is essentially a "reprompt" of the data we were given up front.
//			if (top < 0 || top > 5)
//			{
//				return await stepContext.PromptAsync(nameof(TextPrompt),
//					new PromptOptions
//					{
//						Prompt = repromptMessage,
//					}, cancellationToken);
//			}

//			// return await stepContext.NextAsync(new List<DateTimeResolution> { new DateTimeResolution { Timex = timex } }, cancellationToken);
//			return await stepContext.EndDialogAsync(top, cancellationToken);
//		}

//		//private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//		//{
//		//	var timex = ((List<DateTimeResolution>)stepContext.Result)[0].Timex;
//		//	return await stepContext.EndDialogAsync(timex, cancellationToken);
//		//}

//		//private static Task<bool> DateTimePromptValidator(PromptValidatorContext<IList<DateTimeResolution>> promptContext, CancellationToken cancellationToken)
//		//{
//		//	if (promptContext.Recognized.Succeeded)
//		//	{
//		//		// This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the Time part.
//		//		// TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
//		//		var timex = promptContext.Recognized.Value[0].Timex.Split('T')[0];

//		//		// If this is a definite Date including year, month and day we are good otherwise reprompt.
//		//		// A better solution might be to let the user know what part is actually missing.
//		//		var isDefinite = new TimexProperty(timex).Types.Contains(Constants.TimexTypes.Definite);

//		//		return Task.FromResult(isDefinite);
//		//	}

//		//	return Task.FromResult(false);
//		//}
//	}
//}
