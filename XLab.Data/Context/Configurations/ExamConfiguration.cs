﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using XLab.Data.Context;
using XLab.Data.Models;

namespace XLab.Data.Context.Configurations
{
    public partial class ExamConfiguration : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> entity)
        {
            entity.ToTable("Exam");

            entity.Property(e => e.ExamDate).HasColumnType("datetime");

            entity.HasOne(d => d.Course)
                .WithMany(p => p.Exams)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exam_Course");

            entity.HasOne(d => d.Student)
                .WithMany(p => p.Exams)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exam_Student");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Exam> entity);
    }
}