// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using XLab.Data.Context;
using XLab.Data.Models;

namespace XLab.Data.Context.Configurations
{
    public partial class QuestionAnswerConfiguration : IEntityTypeConfiguration<QuestionAnswer>
    {
        public void Configure(EntityTypeBuilder<QuestionAnswer> entity)
        {
            entity.Property(e => e.Answer)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasOne(d => d.Question)
                .WithMany(p => p.QuestionAnswers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuestionAnswers_Question");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<QuestionAnswer> entity);
    }
}
