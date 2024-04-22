namespace kafkaproducer.services;

public interface IKafkaProducerSvc
{
    void produceMessage(ClientReq eventMsg, string topic);
}