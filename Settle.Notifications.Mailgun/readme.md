# Settle.Notifications.Mailgun

Adds capability to send emails via Mailgun. Requires Settle.Notifications.

Requires configuration options to be set:

```json
"Notifications": {
  "Mailgun": {
    "ApiKey": "<Mailgun api key>",
    "Domain": "<Mailgun email domain>",
    "UseTestModeHeader": false
    "Region": "EU"
  },
    /* Other Notifications settings */
  }
}
```