# Settle.Notifications

## Project overview

This package provides support for email and SMS message sending.

For email sending, it currently supports sending via:

- [Mailgun](https://mailgun.com)

Planned support:

- [Sendgrid](https://sendgrid.com) (Email)
- Twilio (SMS)

## Configuration

Configuration is via the standard configuration files.

| Key | Definition | Required? |
| --- | ---------- | --- |
| defaultSenderEmail | If a sender isn't specified for the message this email is used. This sender must be an approved email on the Mailgun/Sendgrid service | Yes |
| testMode:isEnabled | When in test mode the specified recipients are replaced with the defaultTestRecipient unless the recipient domain or email is in the allowed list | No |
| testMode:defaultRecipient | Specified the receipients to send test messages to | Yes, if in test mode|
| testMode:allowedEmailDomains | Recipient will not be replaced when in test mode if the domain is in this list | No |
| testMode:allowedRecipients | Recipients will not be replaced when in test mode if the email is in this list | No |

```
"notifications": {
    "defaultSenderEmail" : "<email_address>",
    "testMode" {
        "isEnabled" : true|false,
        "defaultRecipient" : "<email_address>[;<email_address>...]",
        "allowedEmailDomains" : "<domain>[;<domain>]",
        "allowedRecipients" : "<email_address>[;<email_address>...]"
    }
}
```

## Test mode

Test mode should be enabled on all pre-production environments to ensure that messages aren't sent to real recipients from those environments. It is possible to enable entire domains or specific email addresses to receive these.

When enabling an entire domain, it is recommended that this should be a specific test domain. For example if your main domain is mydomain.com, then create a subdomain of test.mydomain.com for testing. Accounts required can be aliased in Microsoft Exchange or Cloudflare Email Routing or equivalent.

## Mailgun configuration

If using Mailgun as your email sending provider, install the Mailgun package and set the following configuration values:

| Key | Definition | Required? |
| --- | ---------- | --- |
| Mailgun:ApiKey | the ApiKey for your mailgun account/domain | Yes |
| Mailgun:Domain | Your Mailgun domain | Yes |
| Mailgun:Region | Either EU or US, to indicate the region your domain is based in | No, defaults to EU |
| Mailgun:UseTestModeHeader | Indicates whether to send the o:TestMode header when in test mode - this prevents emails from being sent to any recipient | No |

```
"notifications": {
    "Mailgun": {
      "ApiKey": "<apikey>",
      "Domain": "<domain>",
      "Region": "EU",
      "UseTestModeHeader": true
    }
    ... other notifications setttings
}
```

## Sending your first message

Use the IEmailMessageService to create and send a message. All messages are sent as HTML.

### Parameters

| Parameter | Type | Definition |
| --- | --- | --- |
| recipient | Email | The person to receive the message |
| subject | string | The subject line for the email |
| body | string | The body of the message. |
| tag | string | The tag (category) for the message |
| templateModel | ITemplateModel | an object containing the replacement values for placeholders in the template |
| baseTemplate | string | a baseTemplate for the email. Supports a single placeholder '{{ content }}' which the body is inserted into |
| email | EmailMessage | An email message |

### IEmailMessageService.SendMessageAsync(Email recipient, string subject, string body)

Sends an email to the nominated recipient. This does not support templating of the email message. It will be sent from the default sender, with no tag..

### SendMessageAsync(Email recipient, string subject, string body, string tag)

Sends an email to the nominated recipient. This does not support templating of the email message. It will be sent from the default sender.

### SendMessageWithTemplateAsync(Email recipient, string subject, string body, ITemplateModel templateModel)

Sends an email to the nominated recipient. Supports Liquid templating in the body. Provide an ITemplateModel with the parameters to replace. It will be sent from the default sender, with no tag.

### SendMessageWithTemplateAsync(Email recipient, string subject, string body, ITemplateModel templateModel, string tag)

Sends an email to the nominated recipient. Supports Liquid templating in the body. Provide an ITemplateModel with the parameters to replace. It will be sent from the default sender.

### SendMessageWithBaseTemplateAsync(Email recipient, string subject, string body, string baseTemplate, ITemplateModel templateModel)

Sends an email to the nominated recipient. Supports Liquid templating in the body. Uses a baseTemplate to give an overall layout. Provide an ITemplateModel with the parameters to replace. It will be sent from the default sender, with no tag.

### SendMessageWithBaseTemplateAsync(Email recipient, string subject, string body, string baseTemplate, ITemplateModel templateModel, string tag)

Sends an email to the nominated recipient. Supports Liquid templating in the body. Uses a baseTemplate to give an overall layout. Provide an ITemplateModel with the parameters to replace. It will be sent from the default sender.

### SendMessageAsync(EmailMessage email)

Provides maximum flexibility of options as you can add multiple [TO] recipients and Cc and Bcc recipients, plus specify the sender of the message.