﻿// <auto-generated />
using System;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataAccess.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("DataAccess.Data.REWARD_DATA", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BaseCur");

                    b.Property<double>("Reward");

                    b.Property<string>("SwapCur");

                    b.HasKey("Id");

                    b.ToTable("REWARD_DATA");
                });

            modelBuilder.Entity("DataAccess.Data.TARGET_ORDER", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BaseCur");

                    b.Property<string>("Completed");

                    b.Property<DateTime>("CreateTime");

                    b.Property<string>("NeedSell");

                    b.Property<string>("OrderId");

                    b.Property<double>("Price");

                    b.Property<string>("SellOrderId");

                    b.Property<double>("SwapAmount");

                    b.Property<string>("SwapCur");

                    b.HasKey("Id");

                    b.ToTable("TARGET_ORDER");
                });

            modelBuilder.Entity("DataAccess.SYMBOL_DATA", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SymbolStr");

                    b.HasKey("Id");

                    b.ToTable("SYMBOL_DATA");
                });

            modelBuilder.Entity("DataAccess.TRADE_DATA", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<string>("BaseCur");

                    b.Property<DateTime>("CreateTime");

                    b.Property<double>("Price");

                    b.Property<string>("Side");

                    b.Property<string>("SwapCur");

                    b.Property<string>("Symbol");

                    b.Property<long>("TradeId");

                    b.HasKey("Id");

                    b.ToTable("TRADE_DATA");
                });

            modelBuilder.Entity("DataAccess.TRADE_INFO_PER_MIN", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("AvgPrice");

                    b.Property<string>("BaseCur");

                    b.Property<string>("Gmt8DataTime");

                    b.Property<double>("MaxPrice");

                    b.Property<double>("MinPrice");

                    b.Property<string>("SwapCur");

                    b.Property<string>("Symbol");

                    b.Property<double>("TotalTradeBase");

                    b.HasKey("Id");

                    b.ToTable("TRADE_INFO_PER_MIN");
                });
#pragma warning restore 612, 618
        }
    }
}
