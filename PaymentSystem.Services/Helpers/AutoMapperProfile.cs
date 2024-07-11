using AutoMapper;
using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Card;
using PaymentSystem.DataLayer.EntitiesDTO.Transaction;

namespace PaymentSystem.Services.Helpers
{
    internal class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            ConfigEntitiesMap();
        }
        private void ConfigEntitiesMap()
        {
            MapCard();
            MapTransaction();
        }
        private void MapCard()
        {
            // AddCardDTO -> Card
            CreateMap<AddCardDTO, Card>()
                .ForMember(card => card.Balance, balance => balance.MapFrom(value => 0)); ;

            // EditCardDTO -> Card
            CreateMap<EditCardDTO, Card>();
        }
        private void MapTransaction()
        {
            // AddTransactionDTO -> Transaction
            CreateMap<AddTransactionDTO, Transaction>()
                .ForMember(transaction => transaction.TransactionDate, date => date.MapFrom(value => DateTime.UtcNow));

        }
    }
}
