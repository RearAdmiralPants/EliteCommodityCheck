namespace EliteCommodityAnalysis.Mapping
{
    using Mapster;
    using Abstractions;
    using Abstractions.EDDB;
    using Abstractions.EDDB.JSON;

    public static class MapperConfig
    {

        public static void Configure() {
            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

            TypeAdapterConfig<JSONCommodityCategory, CommodityCategory>.ForType()
                .Map(dest => dest.Id, src => src.id)
                .Map(dest => dest.Name, src => src.name);

            TypeAdapterConfig<JSONCommodity, Commodity>.ForType()
                .Map(dest => dest.Id, src => src.id)
                .Map(dest => dest.IsNonMarketable, src => src.is_non_marketable)
                .Map(dest => dest.IsRare, src => src.is_rare)
                .Map(dest => dest.MaxBuyPrice, src => src.max_buy_price)
                .Map(dest => dest.MaxSellPrice, src => src.max_sell_price)
                .Map(dest => dest.MinBuyPrice, src => src.min_buy_price)
                .Map(dest => dest.MinSellPrice, src => src.min_sell_price)
                .Map(dest => dest.Name, src => src.name)
                .Map(dest => dest.SellPriceUpperAverage, src => src.sell_price_upper_average)
                .Map(dest => dest.AveragePrice, src => src.average_price)
                .Map(dest => dest.BuyPriceLowerAverage, src => src.buy_price_lower_average)
                .Map(dest => dest.CategoryId, src => src.category_id)
                .Map(dest => dest.EdId, src => src.ed_id)
                .Map(dest => dest.Category, src => src.category);

            TypeAdapterConfig<JSONStationState, StationState>.ForType()
                .Map(dest => dest.Id, src => src.id)
                .Map(dest => dest.Name, src => src.name);

            TypeAdapterConfig<JSONStation, Station>.ForType()
                .Map(dest => dest.Id, src => src.id)
                .Map(dest => dest.Name, src => src.name)
                .Map(dest => dest.SystemId, src => src.system_id)
                .Map(dest => dest.UpdatedAt, src => src.updated_at)
                .Map(dest => dest.MaxLandingPadSize, src => src.max_landing_pad_size)
                .Map(dest => dest.DistanceToStar, src => src.distance_to_star)
                .Map(dest => dest.GovernmentId, src => src.government_id)
                .Map(dest => dest.Allegiance, src => src.allegiance)
                .Map(dest => dest.States, src => src.states)
                .Map(dest => dest.TypeId, src => src.type_id)
                .Map(dest => dest.Type, src => src.type)
                .Map(dest => dest.HasBlackmarket, src => src.has_blackmarket)
                .Map(dest => dest.HasMarket, src => src.has_market)
                .Map(dest => dest.HasRefuel, src => src.has_refuel)
                .Map(dest => dest.HasRearm, src => src.has_rearm)
                .Map(dest => dest.HasOutfitting, src => src.has_outfitting)
                .Map(dest => dest.HasShipyard, src => src.has_shipyard)
                .Map(dest => dest.HasDocking, src => src.has_docking)
                .Map(dest => dest.HasCommodities, src => src.has_commodities)
                .Map(dest => dest.HasMaterialTrader, src => src.has_material_trader)
                .Map(dest => dest.HasTechnologyBroker, src => src.has_technology_broker)
                .Map(dest => dest.HasCarrierVendor, src => src.has_carrier_vendor)
                .Map(dest => dest.HasCarrierAdministration, src => src.has_carrier_administration)
                .Map(dest => dest.HasInterstellarFactors, src => src.has_interstellar_factors)
                .Map(dest => dest.HasUniversalCartographics, src => src.has_universal_cartographics)
                .Map(dest => dest.ImportCommodities, src => src.import_commodities)
                .Map(dest => dest.ExportCommodities, src => src.export_commodities)
                .Map(dest => dest.ProhibitedCommodities, src => src.prohibited_commodities)
                .Map(dest => dest.Economies, src => src.economies)
                .Map(dest => dest.ShipyardUpdatedAt, src => src.shipyard_updated_at)
                .Map(dest => dest.OutfittingUpdatedAt, src => src.outfitting_updated_at)
                .Map(dest => dest.MarketUpdatedAt, src => src.market_updated_at)
                .Map(dest => dest.IsPlanetary, src => src.is_planetary)
                .Map(dest => dest.SellingShips, src => src.selling_ships)
                .Map(dest => dest.SellingModules, src => src.selling_modules)
                .Map(dest => dest.SettlementSizeId, src => src.settlement_size_id)
                .Map(dest => dest.SettlementSize, src => src.settlement_size)
                .Map(dest => dest.SettlementSecurityId, src => src.settlement_security_id)
                .Map(dest => dest.SettlementSecurity, src => src.settlement_security)
                .Map(dest => dest.BodyId, src => src.body_id)
                .Map(dest => dest.ControllingMinorFactionId, src => src.controlling_minor_faction_id)
                .Map(dest => dest.EdMarketId, src => src.ed_market_id);

            TypeAdapterConfig<JSONSystemMinorFactionActiveState, SystemMinorFactionState>.ForType()
                .Map(dest => dest.Id, src => src.id)
                .Map(dest => dest.Name, src => src.name)
                .AfterMapping(dest => dest.StrStateType = MinorFactionType.Active.ToString());

            TypeAdapterConfig<JSONSystemMinorFactionPendingState, SystemMinorFactionState>.ForType()
                .Map(dest => dest.Id, src => src.id)
                .Map(dest => dest.Name, src => src.name)
                .AfterMapping(dest => dest.StrStateType = MinorFactionType.Active.ToString());

            TypeAdapterConfig<JSONSystemMinorFactionRecoveringState, SystemMinorFactionState>.ForType()
                //.Map(dest => dest.StrStateType, MinorFacctionType.Recovering.ToString())
                .Map(dest => dest.Id, src => src.id)
                .Map(dest => dest.Name, src => src.name)
                .AfterMapping(dest => dest.StrStateType = MinorFactionType.Active.ToString());

            TypeAdapterConfig<JSONSystemMinorFaction, SystemMinorFaction>.ForType()
                .Map(dest => dest.HappinessId, src => src.happiness_id)
                .Map(dest => dest.MinorFactionId, src => src.minor_faction_id)
                .Map(dest => dest.Influence, src => src.influence)
                .Map(dest => dest.ActiveStates, src => src.active_states)
                .Map(dest => dest.States, src => src.active_states)
                .Map(dest => dest.PendingStates, src => src.pending_states)
                .Map(dest => dest.States, src => src.pending_states)
                .Map(dest => dest.RecoveringStates, src => src.recovering_states)
                .Map(dest => dest.States, src => src.recovering_states)
                .Map(dest => dest.EdSystemAddress, src => src.ed_system_address);

            TypeAdapterConfig<JSONSystemState, SystemState>.ForType()
                .Map(dest => dest.Id, src => src.id)
                .Map(dest => dest.Name, src => src.name);

            TypeAdapterConfig<JSONPopulatedSystem, PopulatedSystem>.ForType()
                .Map(dest => dest.Id, src => src.id)
                .Map(dest => dest.EdsmId, src => src.edsm_id)
                .Map(dest => dest.Name, src => src.name)
                .Map(dest => dest.X, src => src.x)
                .Map(dest => dest.Y, src => src.y)
                .Map(dest => dest.Z, src => src.z)
                .Map(dest => dest.Population, src => src.population)
                .Map(dest => dest.IsPopulated, src => src.is_populated)
                .Map(dest => dest.GovernmentId, src => src.government_id)
                .Map(dest => dest.Government, src => src.government)
                .Map(dest => dest.AllegianceId, src => src.allegiance_id)
                .Map(dest => dest.Allegiance, src => src.allegiance)
                .Map(dest => dest.States, src => src.states)
                .Map(dest => dest.SecurityId, src => src.security_id)
                .Map(dest => dest.Security, src => src.security)
                .Map(dest => dest.PrimaryEconomyId, src => src.primary_economy_id)
                .Map(dest => dest.PrimaryEconomy, src => src.primary_economy)
                .Map(dest => dest.Power, src => src.power)
                .Map(dest => dest.PowerState, src => src.power_state)
                .Map(dest => dest.PowerStateId, src => src.power_state_id)
                .Map(dest => dest.NeedsPermit, src => src.needs_permit)
                .Map(dest => dest.UpdatedAt, src => src.updated_at)
                .Map(dest => dest.MinorFactionsUpdatedAt, src => src.minor_factions_updated_at)
                .Map(dest => dest.SimbadRef, src => src.simbad_ref)
                .Map(dest => dest.ControllingMinorFactionId, src => src.controlling_minor_faction_id)
                .Map(dest => dest.ControllingMinorFaction, src => src.controlling_minor_faction)
                .Map(dest => dest.ReserveTypeId, src => src.reserve_type_id)
                .Map(dest => dest.ReserveType, src => src.reserve_type)
                .Map(dest => dest.MinorFactionPresences, src => src.minor_faction_presences);


        }
    }
}
