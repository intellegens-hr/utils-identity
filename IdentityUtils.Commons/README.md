# IdentityUtils.Commons

This project contains all utility and helper classes which can be used in any other project.

1. [ Intellegens HttpClient ](#httpclient)
2. [ Mailing providers ](#mailingproviders)
3. [ Validation ](#validation)

<a name="httpclient"></a>
## Intellegens HttpClient - IdentityUtils.Commons.IntellegensHttpClient
HttpClient serves as wrapper class around System.Net.HttpClient. To make REST calls easier, following methods are used
- `async Task<T> Get<T>(string url)`
- `async Task<T> Delete<T>(string url)`
- `async Task<T> Post<T>(string url, object dataToSend)`

Each method above uses protected method `GetHttpRequestMessage(HttpMethod method, string url)` to build Request message. This method may be overriden in case additional headers (or any other property) needs to be set.

This client automatically handles JSON serialization/deserialization using Newtonsoft JSON library.

<a name="mailingproviders"></a>
## Mailing providers

### Mailing provider interface

To make sending e-mails more easier, other projects may use any mailing provider which implements `IMailingProvider` interface. `IMailingProvider` interface is simple and specifies only two methods
- `Task SendEmailMessage(MailMessageSimple mailMessageSimple)`
- `Task SendEmailMessage(MailMessage mailMessage)`

Two avoid having multiple libraries with different e-mail message types, `MailMessageSimple` is used as proxy class. `MailMessageSimple` defines all properties typical e-mail message should have:
```csharp
    public class MailMessageSimple
    {
        ...

        public EmailTypes EmailType { get; set; } = EmailTypes.HTML;
        public string From { get; set; }
        public List<string> To { get; set; } = new List<string>();
        public string Subject { get; set; }
        public string Content { get; set; }
    }
```

Static class `Commons.Mailing.MailMessageExtensions` should contain all adapter methods to convert other message types (like `System.Net.Mail.MailMessage`) to `MailMessageSimple`.

### List of mailing providers
- Google (`Commons.Mailing.GoogleMailingProvider`)

## Validation

`ModelValidator` is simple static class which is used to validate any object by using it's data annotation attributes.