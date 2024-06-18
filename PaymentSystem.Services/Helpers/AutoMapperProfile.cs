using AutoMapper;
using PaymentSystem.DataLayer.Entities;
using PaymentSystem.DataLayer.EntitiesDTO.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
        private void MapCard()
        {
            // AddCardDTO -> Card
            CreateMap<AddCardDTO, Card>()
                .ForMember(card => card.Balance, balance => balance.MapFrom(value => 0)); ;

            // EditCardDTO -> Card
            CreateMap<EditCardDTO, Card>();
        }
    }
}
