// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

using CodeFestBot.CognitiveModels;
using CodeFestBot.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeFestBot.Dialogs
{
	public class MainDialog : ComponentDialog
	{
		private readonly TwitterRecognizer _luisRecognizer;
		protected readonly ILogger Logger;
		private readonly IHttpClient _httpExternalClient;
		private const string MESSAGE_DEDICACION = "Puedes preguntarme información sobre del CodeFest, ponencias, y otros temas relacionados.";
		private bool isFirst = true;

		// Dependency injection uses this constructor to instantiate MainDialog
		public MainDialog(TwitterRecognizer luisRecognizer, TwitterDialog twitterDialog, ILogger<MainDialog> logger, IHttpClient httpExternalClient)
			: base(nameof(MainDialog))
		{
			_luisRecognizer = luisRecognizer;
			Logger = logger;
			_httpExternalClient = httpExternalClient;

			AddDialog(new TextPrompt(nameof(TextPrompt)));
			AddDialog(twitterDialog);
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
			{
				IntroStepAsync,
				ActStepAsync,
				// TopNStepAsync,
				// FinalStepAsync
			}));

			// The initial child Dialog to run.
			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			if (!_luisRecognizer.IsConfigured)
			{
				await stepContext.Context.SendActivityAsync(
					MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

				return await stepContext.NextAsync(null, cancellationToken);
			}

			if (isFirst)
			{
				isFirst = false;
				// Use the text provided in FinalStepAsync or the default if it is the first time.
				var messageText = stepContext.Options?.ToString() ?? $"¿Que puedo hacer para ayudarte?{Environment.NewLine}{MESSAGE_DEDICACION}";
				var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
				return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
			}
			return await stepContext.NextAsync(null, cancellationToken);
		}

		private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			if (!_luisRecognizer.IsConfigured)
			{
				// LUIS is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
				return await stepContext.BeginDialogAsync(nameof(TwitterDialog), new TwitterDetails(), cancellationToken);
			}

			// Call LUIS and gather any potential details. (Note the TurnContext has the response to the prompt.)
			var luisResult = await _luisRecognizer.RecognizeAsync<TweetsDetail>(stepContext.Context, cancellationToken);

			switch (luisResult.TopIntent().intent)
			{
				case TweetsDetail.Intent.TopNFunniestTweets:


					var textNumber = luisResult?.Entities?.instance;
					var top = int.Parse(textNumber == null || textNumber.number == null ? "0" : textNumber.number[0].text);

					if (top < 1)
					{
						var topNFunniestMessageText = $"No seas tan generalista. ¡Al menos incluye un número en la pregunta!";
						var topNFunniestMessage = MessageFactory.Text(topNFunniestMessageText, topNFunniestMessageText, InputHints.IgnoringInput);
						await stepContext.Context.SendActivityAsync(topNFunniestMessage, cancellationToken);
					}
					else
					{
						var topNJson = await _httpExternalClient.GetStringAsync($"https://codefesttwitterapi.azurewebsites.net/api/twitter/top?take={top}&orderType=desc");
						var topn = JsonConvert.DeserializeObject<List<Twitter>>(topNJson);

						if (top > 0)
						{
							var title = "Ahí van:";
							var msgTitle = MessageFactory.Text(title, title, InputHints.IgnoringInput);
							await stepContext.Context.SendActivityAsync(msgTitle, cancellationToken);
							foreach (var tweet in topn)
							{
								var msgText = $"{tweet.UserName}: {tweet.Text}";
								var msg = MessageFactory.Text(msgText, msgText, InputHints.IgnoringInput);
								await stepContext.Context.SendActivityAsync(msg, cancellationToken);
							}
						}
					}
					break;

				case TweetsDetail.Intent.NumberOfTweets:
					var numberofTweetsJson = await _httpExternalClient.GetStringAsync($"https://codefesttwitterapi.azurewebsites.net/api/twitter/");
					var list = JsonConvert.DeserializeObject<List<Twitter>>(numberofTweetsJson);

					var numofTweetsMessageText = $"{list.Count} tweets en total !";
					var numofTweetsMessage = MessageFactory.Text(numofTweetsMessageText, numofTweetsMessageText, InputHints.IgnoringInput);
					await stepContext.Context.SendActivityAsync(numofTweetsMessage, cancellationToken);
					break;

				case TweetsDetail.Intent.Ponencias:
					var ponenciasMessageText = $"2 Charlas/Ponencias relacionadas con I.A";
					var ponnenciasMessage = MessageFactory.Text(ponenciasMessageText, ponenciasMessageText, InputHints.IgnoringInput);
					await stepContext.Context.SendActivityAsync(ponnenciasMessage, cancellationToken);
					break;

				case TweetsDetail.Intent.TweetFunniest:
					var funniestJson = await _httpExternalClient.GetStringAsync($"https://codefesttwitterapi.azurewebsites.net/api/Twitter/funniest");
					var funniest = JsonConvert.DeserializeObject<List<Twitter>>(funniestJson).FirstOrDefault();
					var funniestMessageText = $"El Tweet que mas me gusta es el de '{funniest.UserName}'{Environment.NewLine}que dijo: '{funniest.Text}'";
					var funniestMessage = MessageFactory.Text(funniestMessageText, funniestMessageText, InputHints.IgnoringInput);
					await stepContext.Context.SendActivityAsync(funniestMessage, cancellationToken);
					break;
				case TweetsDetail.Intent.TweetSaddest:

					try
					{
						var saddestJson = await _httpExternalClient.GetStringAsync($"https://codefesttwitterapi.azurewebsites.net/api/Twitter/saddest");
						var saddest = JsonConvert.DeserializeObject<List<Twitter>>(saddestJson).FirstOrDefault();

						var saddestMessageText = $"El Tweet mas aburrido es de '{saddest.UserName}'{Environment.NewLine}que dijo: '{saddest.Text}'";
						var saddestMessage = MessageFactory.Text(saddestMessageText, saddestMessageText, InputHints.IgnoringInput);
						await stepContext.Context.SendActivityAsync(saddestMessage, cancellationToken);
					}
					catch (Exception ex)
					{
						throw ex;
					}
					break;


				// case TwitterScoring.Intent.Greetings:
				default:
					var greetingMessageText = $"Disculpa, si sólo quieres conversar habla con mi hermano \"QnA\" que le encantan ese tipo de conversaciones.{Environment.NewLine}{MESSAGE_DEDICACION}";
					var greetingMessage = MessageFactory.Text(greetingMessageText, greetingMessageText, InputHints.IgnoringInput);
					await stepContext.Context.SendActivityAsync(greetingMessage, cancellationToken);
					break;
			}

			return await stepContext.EndDialogAsync(null, cancellationToken);
		}

		private async Task<DialogTurnResult> TopNStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var tweets = (TwitterDetails)stepContext.Options;

			tweets.Top = (int)stepContext.Result;

			if (tweets.Top < 0 || tweets.Top > 5)
			{
				var topNMessageText = $"¿Cuántos tweets quieres leer?";
				var promptMessage = MessageFactory.Text(topNMessageText, topNMessageText, InputHints.ExpectingInput);
				return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
			}

			return await stepContext.NextAsync(tweets.Top, cancellationToken);
		}

		private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			//// If the child dialog ("BookingDialog") was cancelled, the user failed to confirm or if the intent wasn't BookFlight
			//// the Result here will be null.
			//if (stepContext.Result is TwitterDetails result)
			//{
			//	// Now we have all the booking details call the booking service.

			//	// If the call to the booking service was successful tell the user.

			//	// var timeProperty = new TimexProperty(result.TravelDate);
			//	//var travelDateMsg = timeProperty.ToNaturalLanguage(DateTime.Now);
			//	//var messageText = $"I have you booked to {result.Destination} from {result.Origin} on {travelDateMsg}";

			//	// var numerOfTwitterToShow = result.Take;

			//	var messageText = $"Aquí puedes ver el resultado.";
			//	var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
			//	await stepContext.Context.SendActivityAsync(message, cancellationToken);
			//}

			// Restart the main dialog with a different message the second time around
			var promptMessage = "¿Que más puedo hacer por tí?";
			return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
		}
	}
}
