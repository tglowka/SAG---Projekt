For better logging experience during development it is possible to display application logs in the browser using Seq.

Seq is a centralized log server for ingesting and querying structured log events.

To setup Seq server it is required to set value of the "seqserveraddress" within MultiAgentBookingSystem/MultiAgentBookingSystem/App.config to point at local Seq Server instance.

Seq could be setup locally:
 - For Linux: run docker image: https://hub.docker.com/r/datalust/seq
 - For Windows: use https://datalust.co/download