using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Xml;
using bot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json; 

namespace OneTask
{
    public class BotHandler
    {

        private static ITelegramBotClient botClient;
        public static List<long> adminIds = new List<long>();
        public static List<long> personIds = new List<long>();
        public static List<string> categories = new List<string>();
        public static List<Books> books = new List<Books>();
        public static List<string> payTypes = new List<string>();
        public static Books book;
        public string textCRUD;
        public string textCRUDbook;

        public string TITLE;
        public string AUTHOR;
        public string CATEGORY;
        public int YEAR;
        public int PRICE;

        public int row;
        public int bookInNum;
        public bool bookInNumBool = true;

        public bool sendContactMessageCode = false;
        public bool checkContactMessageCode = false;

        // admin
        public bool adminFunc = false;
        public bool categoryCRUD = false;
        public bool bookCRUD = false;
        public bool bookCRUDin = false;
        public bool payTypeCRUD = false;
        public bool orderStatusCRUD = false;
        public bool setOrderStatuses = false;
        public bool downloadTheListOfAllOrders = false;
        public bool downloadTheListOfClients = false;

        public async Task BotStart()
        {
            adminIds.Add(678270125);
            adminIds.Add(1633746526);

            var botClient = new TelegramBotClient("6402801857:AAF-xpZD1lrAp2mGY8usN7tI-2AH4xUMy78");

            using CancellationTokenSource cts = new();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;

            if (message.Text == "/start")
            {
                textCRUD = "";
                textCRUDbook = "";

                bookInNumBool = true;

                sendContactMessageCode = false;
                checkContactMessageCode = false;

                
                adminFunc = false;
                categoryCRUD = false;
                bookCRUD = false;
                bookCRUDin = false;
                payTypeCRUD = false;
                orderStatusCRUD = false;
                setOrderStatuses = false;
                downloadTheListOfAllOrders = false;
                downloadTheListOfClients = false;

                if (personIds.Any(item => message.Chat.Id == item))
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Welcome !!!",
                        cancellationToken: cancellationToken);

                    if (adminIds.Any(item => message.Chat.Id == item))
                    {
                        var oneHandler = update.Type switch
                        {
                            UpdateType.Message => AdminRights(botClient, update, cancellationToken),
                        };
                        return;
                    }
                    else
                    {
                        var oneHandler = update.Type switch
                        {
                            UpdateType.Message => PersonRights(botClient, update, cancellationToken),
                        };
                        return;
                    }
                }
                else
                {
                    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                            KeyboardButton.WithRequestContact("Share Contact"),
                        }
                    )
                    {
                        ResizeKeyboard = true
                    };
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Send Me Contact",
                        replyMarkup: replyKeyboardMarkup,
                        cancellationToken: cancellationToken);
                    sendContactMessageCode = true;
                    return;
                }
            }
            else if (sendContactMessageCode == true)
            {
                if (message.Contact.UserId != message.Chat.Id) 
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Not Your Contact ❌",
                        cancellationToken: cancellationToken);
                    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                        {
                                KeyboardButton.WithRequestContact("Share Contact"),
                            }
                        )
                    {
                        ResizeKeyboard = true
                    };
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Send Me Contact",
                        replyMarkup: replyKeyboardMarkup,
                        cancellationToken: cancellationToken);
                    return;
                }
                else
                {       
                    personIds.Add(message.Chat.Id);

                    // start code

                    // end code

                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Please tell me the code that came to your number !",
                        cancellationToken: cancellationToken);

                    sendContactMessageCode = false;
                    checkContactMessageCode = true;
                    return;
                }
            }
            else if (checkContactMessageCode == true)
            {
                if (message.Text == "0")
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Accepted ✅",
                        cancellationToken: cancellationToken);

                    checkContactMessageCode = false;

                    if (adminIds.Any(item => message.Chat.Id == item))
                    {
                        var oneHandler = update.Type switch
                        {
                            UpdateType.Message => AdminRights(botClient, update, cancellationToken),
                        };
                        return;
                    }
                    else
                    {
                        var oneHandler = update.Type switch
                        {
                            UpdateType.Message => PersonRights(botClient, update, cancellationToken),
                        };
                        return;
                    }

                    return;
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Error Code ❌",
                        cancellationToken: cancellationToken);

                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Please tell me the code that came to your number !",
                        cancellationToken: cancellationToken);
                     
                    return;
                }
            }
            else
            {
                if (adminIds.Any(item => message.Chat.Id == item))
                {
                    var oneHandler = update.Type switch
                    {
                        UpdateType.Message => AdminRights(botClient, update, cancellationToken),
                    };
                    return;
                }
                else
                {
                    var oneHandler = update.Type switch
                    {
                        UpdateType.Message => PersonRights(botClient, update, cancellationToken),
                    };
                    return;
                }
            }
        }
        // ------------------------------- START ADMIN ---------------------------------
        public async Task AdminRights(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;

            if (adminFunc == false)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                        new KeyboardButton[] { "Category CRUD", "Book CRUD" },
                        new KeyboardButton[] { "PayType CRUD", "OrderStatus CRUD" },
                        new KeyboardButton[] { "SetOrderStatuses" },
                        new KeyboardButton[] { "DownloadTheListOfAllOrders" },
                        new KeyboardButton[] { "DownloadTheListOfClients" },
                    }
                )
                {
                    ResizeKeyboard = true
                };

                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Your Are Rights ⬇️",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);

                Console.WriteLine(message.Chat.Id);

                adminFunc = true;
            }
            else if (categoryCRUD == true || message.Text == "Category CRUD")
            {
                var oneHandler = update.Type switch
                {
                    UpdateType.Message => CategoryCRUD(botClient, update, cancellationToken),
                };
                return;
            }
            else if (bookCRUD == true || message.Text == "Book CRUD")
            {
                var oneHandler = update.Type switch
                {
                    UpdateType.Message => BookCRUD(botClient, update, cancellationToken),
                };
                return;
            }
            else if (payTypeCRUD == true || message.Text == "PayType CRUD")
            {
                var oneHandler = update.Type switch
                {
                    UpdateType.Message => PayTypeCRUD(botClient, update, cancellationToken),
                };
                return;
            }
            else if (orderStatusCRUD == true || message.Text == "OrderStatus CRUD")
            {
                var oneHandler = update.Type switch
                {
                    UpdateType.Message => OrderStatusCRUD(botClient, update, cancellationToken),
                };
                return;
            }
            else if (setOrderStatuses == true || message.Text == "SetOrderStatuses")
            {
                var oneHandler = update.Type switch
                {
                    UpdateType.Message => SetOrderStatuses(botClient, update, cancellationToken),
                };
                return;
            }
            else if (downloadTheListOfAllOrders == true || message.Text == "DownloadTheListOfAllOrders")
            {
                var oneHandler = update.Type switch
                {
                    UpdateType.Message => DownloadTheListOfAllOrders(botClient, update, cancellationToken),
                };
                return;
            }
            else if (downloadTheListOfClients == true || message.Text == "DownloadTheListOfClients")
            {
                var oneHandler = update.Type switch
                {
                    UpdateType.Message => DownloadTheListOfClients(botClient, update, cancellationToken),
                };
                return;
            }
        }
        public async Task CategoryCRUD(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;

            if (categoryCRUD == false)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                        new KeyboardButton[] { "Category Create", "Category Read" },
                        new KeyboardButton[] { "Category Update", "Category Delete" },
                        new KeyboardButton[] { "Back ⬅️"},
                }
                )
                {
                    ResizeKeyboard = true
                };

                await botClient.SendTextMessageAsync(
                     chatId: message.Chat.Id,
                     text: "Choose One ⬇️",
                     replyMarkup: replyKeyboardMarkup,
                     cancellationToken: cancellationToken);

                Console.WriteLine(message.Chat.Id);

                categoryCRUD = true;
            }
            else if (message.Text == "Category Create")
            {
                textCRUD = "Category Create";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Send New Category",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (textCRUD == "Category Create")
            {
                textCRUD = "";
                if (categories.Any(item => message.Text == item))
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Same Category ❌",
                        cancellationToken: cancellationToken);
                    return;
                }
                else
                {
                    categories.Add(message.Text);
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Accepted New Category ✅",
                        cancellationToken: cancellationToken);
                    return;
                }
            }
            else if (message.Text == "Category Read")
            {
                if (categories.Count > 0)
                {
                    string sendText = "\t\tCategories ⬇️\n\n";
                    int num = 1;
                    foreach (string category in categories)
                    {
                        sendText += $"{num}  ➡️  {category}\n";
                        num++;
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: sendText,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No Category",
                        cancellationToken: cancellationToken);
                }
                return;
            }
            else if (message.Text == "Category Update")
            {
                if (categories.Count > 0)
                {
                    string sendText = "\t\tCategories ⬇️\n\n";
                    int num = 1;
                    foreach (string category in categories)
                    {
                        sendText += $"{num}  ➡️  {category}\n";
                        num++;
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: sendText,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No Category",
                        cancellationToken: cancellationToken);
                }
                textCRUD = "Category Update";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Which category do you want to update?",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (textCRUD == "Category Update")
            {
                if (categories.Count > 0)
                {
                    try
                    {
                        int num = 0;
                        for (int i = 0; i < categories.Count; i++)
                        {
                            if (i+1 == int.Parse(message.Text))
                            {
                                row = int.Parse(message.Text);
                                textCRUD = "To Write";
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    text: "To Write",
                                    cancellationToken: cancellationToken);
                                num = 1;
                                break;
                            }
                        }
                        if (num != 1)
                        {
                            textCRUD = "Category Update";
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "This number does not exist in the category ?!",
                                cancellationToken: cancellationToken);
                        }
                    }
                    catch
                    {                        
                        textCRUD = "Category Update";
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "It is not number ❌",
                            cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No Category",
                        cancellationToken: cancellationToken);
                }
                return;
            }
            else if (textCRUD == "To Write")
            {
                textCRUD = "";
                for (int i = 0; i < categories.Count; i++)
                {
                    if (i+1 == row || categories[i] == message.Text)
                    {
                        if (categories[i] == message.Text)
                        {
                            textCRUD = "To Write";
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "Same Category ❌",
                                cancellationToken: cancellationToken);
                            return;
                        }
                        else
                        {
                            categories[i] = message.Text;
                        }
                    }
                }
                textCRUD = "";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"{row} ➡️ Row Updated",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (message.Text == "Category Delete")
            {
                if (categories.Count > 0)
                {
                    string sendText = "\t\tCategories ⬇️\n\n";
                    int num = 1;
                    foreach (string category in categories)
                    {
                        sendText += $"{num}  ➡️  {category}\n";
                        num++;
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: sendText,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No Category",
                        cancellationToken: cancellationToken);
                }
                textCRUD = "Category Delete";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Which category do you want to clear?",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (textCRUD == "Category Delete")
            {
                if (categories.Count > 0)
                {
                    try
                    {
                        int num = 0;
                        for (int i = 0; i < categories.Count; i++)
                        {
                            if (i+1 == int.Parse(message.Text))
                            {
                                textCRUD = "";
                                categories.RemoveAt(i);
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    text: $"{i+1}  ➡️  Delete Category 🚮",
                                    cancellationToken: cancellationToken);
                                num = 1;
                                break;
                            }
                        }
                        if (num != 1)
                        {
                            textCRUD = "Category Delete";
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "This number does not exist in the category ?!",
                                cancellationToken: cancellationToken);
                        }
                    }
                    catch
                    {
                        textCRUD = "Category Delete";
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "It is not number ❌",
                            cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No Category",
                        cancellationToken: cancellationToken);
                }
                return;
            }
            else if (message.Text == "Back ⬅️")
            {
                categoryCRUD = false;
                adminFunc = false;
                textCRUD = "";
                var oneHandler = update.Type switch
                {
                    UpdateType.Message => AdminRights(botClient, update, cancellationToken),
                };

                return;
            }
        }
        public async Task BookCRUD(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;

            if (bookCRUD == false)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                        new KeyboardButton[] { "Book Create", "Book Read" },
                        new KeyboardButton[] { "Book Update", "Book Delete" },
                        new KeyboardButton[] { "Back ⬅️"},
                }
                )
                {
                    ResizeKeyboard = true
                };

                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose One ⬇️",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);

                Console.WriteLine(message.Chat.Id);

                bookCRUD = true;
            }
            else if (message.Text == "Book Create")
            {
                if (categories.Count > 0)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Book Create",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "To Write Book Title",
                        cancellationToken: cancellationToken);
                    textCRUD = "Book Create1";
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No Categories ❌",
                        cancellationToken: cancellationToken);
                    textCRUD = "";
                }
            }
            else if (textCRUD == "Book Create1")
            {
                TITLE = message.Text;
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Accepted New Book Title ✅",
                    cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "To Write Book Author",
                    cancellationToken: cancellationToken);
                textCRUD = "Book Create2";
            }
            else if (textCRUD == "Book Create2")
            {
                AUTHOR = message.Text;
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Accepted New Book Author ✅",
                    cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "To Write Book Category",
                    cancellationToken: cancellationToken);
                textCRUD = "Book Create3";
            }
            else if (textCRUD == "Book Create3")
            {
                if (categories.Any(item => message.Text == item))
                {
                    CATEGORY = message.Text;
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Accepted New Book Category ✅",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "To Write Book Year",
                        cancellationToken: cancellationToken);
                    textCRUD = "Book Create4";
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No such category exists ❌",
                        cancellationToken: cancellationToken);
                    string sendText = "\t\tCategories ⬇️\n\n";
                    int num = 1;
                    foreach (string category in categories)
                    {
                        sendText += $"{num}  ➡️  {category}\n";
                        num++;
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: sendText,
                        cancellationToken: cancellationToken);
                }
            }
            else if (textCRUD == "Book Create4")
            {
                try
                {
                    YEAR = int.Parse(message.Text);
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Accepted New Book Year ✅",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "To Write Book Price",
                        cancellationToken: cancellationToken);
                    textCRUD = "Book Create5";
                }
                catch
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Not Number ❌",
                        cancellationToken: cancellationToken);
                    return;
                }
            }
            else if (textCRUD == "Book Create5")
            {
                try
                {
                    PRICE = int.Parse(message.Text);
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Accepted New Book Price ✅",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Book Saved ✅",
                        cancellationToken: cancellationToken);
                    book = new Books();
                    book.title = TITLE;
                    book.author = AUTHOR;
                    book.category = CATEGORY;
                    book.year = YEAR;
                    book.price = PRICE;
                    books.Add(book);
                    textCRUD = "";
                }
                catch
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Not Number ❌",
                        cancellationToken: cancellationToken);
                    return;
                }
            }
            else if (message.Text == "Book Read")
            {
                if (books.Count > 0)
                {
                    textCRUD = "";
                    string sendText = "      Books ⬇️\n";
                    int num = 1;
                    foreach (Books kitob in books)
                    {
                        sendText += $"\n{num}   Book\n";
                        sendText += $"     Title ➡️ {kitob.title}\n";
                        sendText += $"     Author ➡️ {kitob.author}\n";
                        sendText += $"     Category ➡️ {kitob.category}\n";
                        sendText += $"     Year ➡️ {kitob.year}\n";
                        sendText += $"     Price ➡️ {kitob.price}\n";
                        num++;
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: sendText,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No Books",
                        cancellationToken: cancellationToken);
                }
                return;
            }
            else if (message.Text == "Book Update")
            {
                if (books.Count > 0)
                {
                    string sendText = "Books ⬇️\n\n";
                    int num = 1;
                    foreach (Books kitob in books)
                    {
                        sendText += $"\n{num}   Book\n";
                        sendText += $"     Title ➡️ {kitob.title}\n";
                        sendText += $"     Author ➡️ {kitob.author}\n";
                        sendText += $"     Category ➡️ {kitob.category}\n";
                        sendText += $"     Year ➡️ {kitob.year}\n";
                        sendText += $"     Price ➡️ {kitob.price}\n";
                        num++;
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: sendText,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No Books",
                        cancellationToken: cancellationToken);
                    return;
                }
                textCRUD = "Book Update";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Which book you want to change?",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (textCRUD == "Book Update")
            {
                if (books.Count > 0)
                {
                    try
                    {
                        int num = 0;
                        if (bookInNumBool == true)
                        {
                            bookInNum = int.Parse(message.Text);
                        }
                        for (int i = 0; i < books.Count; i++)
                        {
                            if (i + 1 == bookInNum)
                            {
                                bookInNumBool = false;
                                textCRUD = "Book Update";
                                var oneHandler = update.Type switch
                                {
                                    UpdateType.Message => BookCRUDin(botClient, update, cancellationToken),
                                };
                                return;
                            }
                        }
                        if (num != 1)
                        {
                            textCRUD = "Book Update";
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "This number does not exist in the book ?!",
                                cancellationToken: cancellationToken);
                        }
                    }
                    catch
                    {
                        textCRUD = "Book Delete";
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "It is not number ❌",
                            cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No Book",
                        cancellationToken: cancellationToken);
                }
                return;
            }
            else if (message.Text == "Book Delete")
            {
                if (books.Count > 0)
                {
                    string sendText = "Books ⬇️\n\n";
                    int num = 1; 
                    foreach (Books kitob in books)
                    {
                        sendText += $"\n{num}   Book\n";
                        sendText += $"     Title ➡️ {kitob.title}\n";
                        sendText += $"     Author ➡️ {kitob.author}\n";
                        sendText += $"     Category ➡️ {kitob.category}\n";
                        sendText += $"     Year ➡️ {kitob.year}\n";
                        sendText += $"     Price ➡️ {kitob.price}\n";
                        num++;
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: sendText,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No Books",
                        cancellationToken: cancellationToken);
                    return;
                }
                textCRUD = "Book Delete";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Which book do you want to clear?",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (textCRUD == "Book Delete")
            {
                if (books.Count > 0)
                {
                    try
                    {
                        int num = 0;
                        for (int i = 0; i < books.Count; i++)
                        {
                            if (i + 1 == int.Parse(message.Text))
                            {
                                textCRUD = "";
                                books.RemoveAt(i);
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    text: $"{i + 1}  ➡️  Delete Book 🚮",
                                    cancellationToken: cancellationToken);
                                num = 1;
                                break;
                            }
                        }
                        if (num != 1)
                        {
                            textCRUD = "Book Delete";
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "This number does not exist in the book ?!",
                                cancellationToken: cancellationToken);
                        }
                    }
                    catch
                    {
                        textCRUD = "Book Delete";
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "It is not number ❌",
                            cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No Book",
                        cancellationToken: cancellationToken);
                }
                return;
            }
            else if (message.Text == "Back ⬅️")
            {
                bookCRUD = false;
                adminFunc = false;
                textCRUD = "";
                var oneHandler = update.Type switch
                {
                    UpdateType.Message => AdminRights(botClient, update, cancellationToken),
                };

                return;
            }
        }
        public async Task BookCRUDin(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;

            if (bookCRUDin == false)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                        new KeyboardButton[] { "Book Title Update", "Book Author Update" },
                        new KeyboardButton[] { "Book Category Update", "Book Year Update" },
                        new KeyboardButton[] { "Book Price Update" },
                        new KeyboardButton[] { "Back ⬅️"},
                }
                )
                {
                    ResizeKeyboard = true
                };

                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose One ⬇️",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);

                Console.WriteLine(message.Chat.Id);

                bookCRUDin = true;
            }
            else if (message.Text == "Book Title Update")
            {
                textCRUDbook = "Book Title Update";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "To Write",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (textCRUD == "Book Title Update")
            {
                textCRUDbook = "";
                for (int i = 0; i < books.Count; i++)
                {
                    if (i+1 == bookInNum)
                    {
                        books[i].title = message.Text; 
                        break;
                    }
                }
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Accepted Book New Title ✅",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (message.Text == "Book Author Update")
            {
                textCRUDbook = "Book Author Update";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "To Write",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (textCRUD == "Book Author Update")
            {
                textCRUDbook = "";
                for (int i = 0; i < books.Count; i++)
                {
                    if (i + 1 == bookInNum)
                    {
                        books[i].author = message.Text;
                        break;
                    }
                }
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Accepted Book New Author ✅",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (message.Text == "Book Category Update")
            {
                textCRUDbook = "Book Category Update";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "To Write",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (textCRUDbook == "Book Category Update")
            {
                if (categories.Any(item => message.Text == item))
                {
                    for (int i = 0; i < books.Count; i++)
                    {
                        if (i + 1 == bookInNum)
                        {
                            books[i].category = message.Text;
                            break;
                        }
                    }
                    textCRUDbook = "";
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Accepted Book New Category ✅",
                        cancellationToken: cancellationToken);
                }
                else
                {
                    textCRUDbook = "";
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No such category exists ❌",
                        cancellationToken: cancellationToken);
                    string sendText = "\t\tCategories ⬇️\n\n";
                    int num = 1;
                    foreach (string category in categories)
                    {
                        sendText += $"{num}  ➡️  {category}\n";
                        num++;
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: sendText,
                        cancellationToken: cancellationToken);
                }
                return;
            }
            else if (message.Text == "Book Year Update")
            {
                textCRUDbook = "Book Year Update";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "To Write",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (textCRUDbook == "Book Year Update")
            {
                try
                {
                    for (int i = 0; i < books.Count; i++)
                    {
                        if (i + 1 == bookInNum)
                        {
                            books[i].year = int.Parse(message.Text);
                            break;
                        }
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Accepted Book New Year ✅",
                        cancellationToken: cancellationToken);
                    textCRUDbook = "";
                }
                catch
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Not Number ❌",
                        cancellationToken: cancellationToken);
                    return;
                }
            }
            else if (message.Text == "Book Price Update")
            {
                textCRUDbook = "Book Price Update";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "To Write",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (textCRUDbook == "Book Price Update")
            {
                try
                {
                    for (int i = 0; i < books.Count; i++)
                    {
                        if (i + 1 == bookInNum)
                        {
                            books[i].price = int.Parse(message.Text);
                            break;
                        }
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Accepted Book New Price ✅",
                        cancellationToken: cancellationToken);
                    textCRUDbook = "";
                }
                catch
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Not Number ❌",
                        cancellationToken: cancellationToken);
                    return;
                }
            }
            else if (message.Text == "Back ⬅️")
            {
                bookCRUDin = false;
                bookCRUD = false;
                bookInNumBool = true;
                textCRUD = "";
                textCRUDbook = "";
                var oneHandler = update.Type switch
                {
                    UpdateType.Message => BookCRUD(botClient, update, cancellationToken),
                };
                return;
            }
        }
        public async Task PayTypeCRUD(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;

            if (payTypeCRUD == false)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                        new KeyboardButton[] { "PayType Create", "PayType Read" },
                        new KeyboardButton[] { "PayType Update", "PayType Delete" },
                        new KeyboardButton[] { "Back ⬅️"},
                }
                )
                {
                    ResizeKeyboard = true
                };

                await botClient.SendTextMessageAsync(
                     chatId: message.Chat.Id,
                     text: "Choose One ⬇️",
                     replyMarkup: replyKeyboardMarkup,
                     cancellationToken: cancellationToken);

                Console.WriteLine(message.Chat.Id);

                payTypeCRUD = true;
            }
            else if (message.Text == "PayType Create")
            {
                textCRUD = "PayType Create";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Send New PayType",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (textCRUD == "PayType Create")
            {
                textCRUD = "";
                if (payTypes.Any(item => message.Text == item))
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Same PayType ❌",
                        cancellationToken: cancellationToken);
                    return;
                }
                else
                {
                    payTypes.Add(message.Text);
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Accepted New PayType ✅",
                        cancellationToken: cancellationToken);
                    return;
                }
            }
            else if (message.Text == "PayType Read")
            {
                if (payTypes.Count > 0)
                {
                    string sendText = "   PayTypes ⬇️\n\n";
                    int num = 1;
                    foreach (string payType in payTypes)
                    {
                        sendText += $"{num}  ➡️  {payType}\n";
                        num++;
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: sendText,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No PayType",
                        cancellationToken: cancellationToken);
                }
                return;
            }
            else if (message.Text == "PayType Update")
            {
                if (payTypes.Count > 0)
                {
                    string sendText = "    PayType ⬇️\n\n";
                    int num = 1;
                    foreach (string payType in payTypes)
                    {
                        sendText += $"{num}  ➡️  {payType}\n";
                        num++;
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: sendText,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No PayType",
                        cancellationToken: cancellationToken);
                }
                textCRUD = "PayType Update";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Which PayType do you want to update?",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (textCRUD == "PayType Update")
            {
                if (payTypes.Count > 0)
                {
                    try
                    {
                        int num = 0;
                        for (int i = 0; i < payTypes.Count; i++)
                        {
                            if (i + 1 == int.Parse(message.Text))
                            {
                                row = int.Parse(message.Text);
                                textCRUD = "To Write";
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    text: "To Write",
                                    cancellationToken: cancellationToken);
                                num = 1;
                                break;
                            }
                        }
                        if (num != 1)
                        {
                            textCRUD = "PayType Update";
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "This number does not exist in the payType ?!",
                                cancellationToken: cancellationToken);
                        }
                    }
                    catch
                    {
                        textCRUD = "Category Update";
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "It is not number ❌",
                            cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No PayType",
                        cancellationToken: cancellationToken);
                }
                return;
            }
            else if (textCRUD == "To Write")
            {
                textCRUD = "";
                for (int i = 0; i < payTypes.Count; i++)
                {
                    if (i + 1 == row || payTypes[i] == message.Text)
                    {
                        if (payTypes[i] == message.Text)
                        {
                            textCRUD = "To Write";
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "Same PayType ❌",
                                cancellationToken: cancellationToken);
                            return;
                        }
                        else
                        {
                            payTypes[i] = message.Text;
                        }
                    }
                }
                textCRUD = "";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"{row} ➡️ Row Updated",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (message.Text == "PayType Delete")
            {
                if (payTypes.Count > 0)
                {
                    string sendText = "PayTypes ⬇️\n\n";
                    int num = 1;
                    foreach (string payType in payTypes)
                    {
                        sendText += $"{num}  ➡️  {payType}\n";
                        num++;
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: sendText,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No PayType",
                        cancellationToken: cancellationToken);
                }
                textCRUD = "PayType Delete";
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Which PayType do you want to clear ?",
                    cancellationToken: cancellationToken);
                return;
            }
            else if (textCRUD == "PayType Delete")
            {
                if (payTypes.Count > 0)
                {
                    try
                    {
                        int num = 0;
                        for (int i = 0; i < payTypes.Count; i++)
                        {
                            if (i + 1 == int.Parse(message.Text))
                            {
                                textCRUD = "";
                                payTypes.RemoveAt(i);
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    text: $"{i + 1}  ➡️  Delete PayType 🚮",
                                    cancellationToken: cancellationToken);
                                num = 1;
                                break;
                            }
                        }
                        if (num != 1)
                        {
                            textCRUD = "PayType Delete";
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "This number does not exist in the category ?!",
                                cancellationToken: cancellationToken);
                        }
                    }
                    catch
                    {
                        textCRUD = "PayType Delete";
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "It is not number ❌",
                            cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "No Category",
                        cancellationToken: cancellationToken);
                }
                return;
            }
            else if (message.Text == "Back ⬅️")
            {
                payTypeCRUD = false;
                adminFunc = false;
                textCRUD = "";
                var oneHandler = update.Type switch
                {
                    UpdateType.Message => AdminRights(botClient, update, cancellationToken),
                };

                return;
            }
        }
        public async Task OrderStatusCRUD(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;

            
            if (orderStatusCRUD == false)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                        new KeyboardButton[] { "OrderStatus Create", "OrderStatus Read" },
                        new KeyboardButton[] { "OrderStatus Update", "OrderStatus Delete" },
                        new KeyboardButton[] { "Back ⬅️"},
                }
                )
                {
                    ResizeKeyboard = true
                };

                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose One ⬇️",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);

                Console.WriteLine(message.Chat.Id);

                orderStatusCRUD = true;
            }
            else if (message.Text == "OrderStatus Create")
            {
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "OrderStatus Create",
                    cancellationToken: cancellationToken);
            }
            else if (message.Text == "OrderStatus Read")
            {
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "OrderStatus Read",
                    cancellationToken: cancellationToken);
            }
            else if (message.Text == "OrderStatus Update")
            {
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "OrderStatus Update",
                    cancellationToken: cancellationToken);
            }
            else if (message.Text == "OrderStatus Delete")
            {
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "OrderStatus Delete",
                    cancellationToken: cancellationToken);
            }
            else if (message.Text == "Back ⬅️")
            {
                orderStatusCRUD = false;
                adminFunc = false;

                var oneHandler = update.Type switch
                {
                    UpdateType.Message => AdminRights(botClient, update, cancellationToken),
                };

                return;
            }
        }
        public async Task SetOrderStatuses(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

        }
        public async Task DownloadTheListOfAllOrders(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

        }
        public async Task DownloadTheListOfClients(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            /*IronPdf.License.IsValidLicense("IRONSUITE.GAMESUPRO01.GMAIL.COM.30313-D398AC482D-JW43S-QUDWCEWXFEBL-SD7GQBJPVGMX-V4NB4P5UQSMC-F3KNIOKB6CQV-MRWWG4DUPIZV-ABK4IM27LLRV-EMAKMU-TDXQQBLNC2SLUA-DEPLOYMENT.TRIAL-L4RNPU.TRIAL.EXPIRES.05.MAR.2024");

            string text = IO.File.ReadAllText(FilePath + ".json");
            ChromePdfRenderer renderer = new ChromePdfRenderer();
            PdfDocument pdf = renderer.RenderHtmlAsPdf(text);
            pdf.SaveAs(FilePath + ".pdf");

            await using Stream stream = System.IO.File.OpenRead(FilePath + ".pdf");
            await botClient.SendDocumentAsync(
                chatId: message.Chat.Id,
                document: InputFile.FromStream(stream: stream, fileName: $"datas.pdf"),
                caption: "Foydanaluvchilar ma'lumotlari"
                );
            stream.Dispose();*/

        }
        // ------------------------------- END ADMIN ---------------------------------
        // ------------------------------- START PERSON ---------------------------------

        public async Task PersonRights(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Diyor Vo'rrrr 👌🤣",
                cancellationToken: cancellationToken);
        }
        // ------------------------------- END PERSON ---------------------------------
        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
        }
    }
}