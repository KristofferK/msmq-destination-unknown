# msmq-destination-unknown
Using MSMQ Subsystem A must send a message to Subsystem B without either knowing the address of the other subsystem.

# The flow
* Subsystem A sends a request-reply message to Subsystem C asking for Subsystem B's address.
  * Subsystem A will provide a Return Address, so Subsystem C knows where to send the reply.
* Subsystem A will now send four messages to Subsystem B.
  * Message 1 and 2 will have the same correlation id.
  * Message 3 and 4 will have the same correlation id.
  * The messages will contain a integer that Subsystem B can sum (aggregation).
* Subsystem B will reply with two messages to Subsystem A.
  * Message 1 will contain the sum of message 1 and 2. It will have the same correlation id as those.
  * Message 2 will contain the sum of message 3 and 4. It will have the same correlation id as those.