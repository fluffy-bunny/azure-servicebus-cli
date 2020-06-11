# azure-servicebus-cli

## Setting the Stage

Create ServiceBus Namespace  
```
scalesets
```

Create queue (DON’T enable Sessions) 
```
w10rs5pr0
```
  Set the lock time to be 5 minutes (MAX)  
  Store the SAS Policy Keys in KeyVault
```
RootManageSharedAccessKey
  Primary Key: **REDACTED**  
  Secondary Key: **REDACTED**  
  Primary Connection String: **REDACTED**  
  Secondary Connection String: **REDACTED**  
```
When you automate generating a SAS token you can pull the ```Primary-Key``` from keyvault.  

## This is a managed identity CLI
Make sure your identity has access to the service-bus in question.  
```
az login
```
## queue settings  

The ```service-bus-settings``` command stores the queue information locally so that we don't all subsequent commands work on that setting.  
```
ServiceBusCLI service-bus-settings -n scalesets -q w10rs5pr0
```
## SecurityAccessSignature Settings  
Next we want to store locally the credentials we need to generate a SAS token using the ```generate-sas``` command.  
```
  generate-sas -k <primary-key> -p RootManageSharedAccessKey -e 3600 -s
```
This is needed so a downstream command ```renew-lock``` can work.  

## Send A Job Message 
```
send-job
```
This is hard coded to send a job json to the service-bus.  

## Receive a Job
```
receive
```
### output  
```
{
  "Message": {
    "Body": "eyJJZCI6IjNjOGMwMjY4LWFiZDYtNDQ4Mi04NGYwLTJjMjA1OTVkZTNmZCIsIk5hbWUiOiJNeSBTdXBlckR1cGVyIEpvYiIsIklzc3VlZFRpbWUiOiIyMDIwLTA2LTExVDE0OjM1OjQwLjQyNTYyMjJaIn0=",
    "MessageId": "100172c6b7b94370bae1ae29399b2631",
    "PartitionKey": null,
    "ViaPartitionKey": null,
    "SessionId": null,
    "ReplyToSessionId": null,
    "ExpiresAtUtc": "2020-06-12T14:36:19.721Z",
    "TimeToLive": {
      "Ticks": 864000000000,
      "Days": 1,
      "Hours": 0,
      "Milliseconds": 0,
      "Minutes": 0,
      "Seconds": 0,
      "TotalDays": 1,
      "TotalHours": 24,
      "TotalMilliseconds": 86400000,
      "TotalMinutes": 1440,
      "TotalSeconds": 86400
    },
    "CorrelationId": null,
    "Label": null,
    "To": null,
    "ContentType": null,
    "ReplyTo": null,
    "ScheduledEnqueueTimeUtc": "0001-01-01T00:00:00",
    "Size": 116,
    "UserProperties": {},
    "SystemProperties": {
      "IsLockTokenSet": true,
      "LockToken": "d5d57f8d-ca3b-45fb-9f49-ec5a3d272a6f",
      "IsReceived": true,
      "DeliveryCount": 1,
      "LockedUntilUtc": "2020-06-11T22:36:39.553Z",
      "SequenceNumber": 13,
      "DeadLetterSource": null,
      "EnqueuedSequenceNumber": 0,
      "EnqueuedTimeUtc": "2020-06-11T14:36:19.721Z"
    }
  },
  "Job": {
    "Id": "3c8c0268-abd6-4482-84f0-2c20595de3fd",
    "Name": "My SuperDuper Job",
    "IssuedTime": "2020-06-11T14:35:40.4256222Z"
  }
}
```
You will notice the following;  
```"MessageId": "100172c6b7b94370bae1ae29399b2631"```  
and  
```"LockToken": "d5d57f8d-ca3b-45fb-9f49-ec5a3d272a6f"```  
## Renew-Lock
```
ServiceBusCLI renew-lock -m 100172c6b7b94370bae1ae29399b2631 -l d5d57f8d-ca3b-45fb-9f49-ec5a3d272a6f
```
This will slide out the lock by whatever you configured the lock-timeout to be.   In my case 5 minutes.  
This demonstrates that a lock can be renewed by anyone as long as you have the credentials to do so.  
