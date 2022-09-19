using Microsoft.EntityFrameworkCore;
using Reference_Aids.Models;

namespace Reference_Aids.Data
{
    public partial class Reference_AIDSContext : DbContext
    {
        public Reference_AIDSContext()
        {
        }

        public Reference_AIDSContext(DbContextOptions<Reference_AIDSContext> options) : base(options)
        {
        }

        public virtual DbSet<ListCategory> ListCategories { get; set; } = null!;
        public virtual DbSet<ListQualitySerum> ListQualitySerums { get; set; } = null!;
        public virtual DbSet<ListResult> ListResults { get; set; } = null!;
        public virtual DbSet<ListSendDepartment> ListSendDepartments { get; set; } = null!;
        public virtual DbSet<ListSendDistrict> ListSendDistricts { get; set; } = null!;
        public virtual DbSet<ListSendLab> ListSendLabs { get; set; } = null!;
        public virtual DbSet<TblIncomingBlood> TblIncomingBloods { get; set; } = null!;
        public virtual DbSet<TblPatientCard> TblPatientCards { get; set; } = null!;
        public virtual DbSet<TblResultAntigen> TblResultAntigens { get; set; } = null!;
        public virtual DbSet<TblResultBlot> TblResultBlots { get; set; } = null!;
        public virtual DbSet<TblResultIfa> TblResultIfas { get; set; } = null!;
        public virtual DbSet<TblResultPcr> TblResultPcrs { get; set; } = null!;
        public virtual DbSet<TblUser> TblUsers { get; set; } = null!;
        public virtual DbSet<ListSex> ListSexes { get; set; } = null!;
        public virtual DbSet<TblDistrictBlot> TblDistrictBlots { get; set; } = null!;
        public virtual DbSet<ListRegion> ListRegions { get; set; } = null!;
        public virtual DbSet<ListTestSystem> ListTestSystems { get; set; } = null!;
        public virtual DbSet<ListTypeAntigen> ListTypeAntigens { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=Reference_AIDS;Username=vs_test;Password=4100");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ListCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("list_category_pkey");

                entity.ToTable("list_category");

                entity.Property(e => e.CategoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("category_id");

                entity.Property(e => e.RowNum)
                    .HasColumnType("integer")
                    .HasColumnName("row_num");

                entity.Property(e => e.CategoryName)
                    .HasColumnType("character varying")
                    .HasColumnName("category_name");
            });

            modelBuilder.Entity<ListQualitySerum>(entity =>
            {
                entity.HasKey(e => e.QualitySerumId)
                    .HasName("list_quality_serum_pkey");

                entity.ToTable("list_quality_serum");

                entity.Property(e => e.QualitySerumId)
                    .ValueGeneratedNever()
                    .HasColumnName("quality_serum_id");

                entity.Property(e => e.QualitySerumName)
                    .HasColumnType("character varying")
                    .HasColumnName("quality_serum_name");
            });

            modelBuilder.Entity<ListResult>(entity =>
            {
                entity.HasKey(e => e.ResultId)
                    .HasName("list_result_pkey");

                entity.ToTable("list_result");

                entity.Property(e => e.ResultId)
                    .ValueGeneratedNever()
                    .HasColumnName("result_id");

                entity.Property(e => e.ResultName)
                    .HasColumnType("character varying")
                    .HasColumnName("result_name");
            });

            modelBuilder.Entity<ListSendDepartment>(entity =>
            {
                entity.HasKey(e => e.SendDepartmentId)
                    .HasName("list_send_department_pkey");

                entity.ToTable("list_send_department");

                entity.Property(e => e.SendDepartmentId).HasColumnName("send_department_id");

                entity.Property(e => e.SendDepartmentName)
                    .HasColumnType("character varying")
                    .HasColumnName("send_department_name");
            });

            modelBuilder.Entity<ListSendDistrict>(entity =>
            {
                entity.HasKey(e => e.SendDistrictId)
                    .HasName("list_send_district_pkey");

                entity.ToTable("list_send_district");

                entity.Property(e => e.SendDistrictId).HasColumnName("send_district_id");

                entity.Property(e => e.SendDistrictName)
                    .HasColumnType("character varying")
                    .HasColumnName("send_district_name");
            });

            modelBuilder.Entity<ListSendLab>(entity =>
            {
                entity.HasKey(e => e.SendLabId)
                    .HasName("list_send_lab_pkey");

                entity.ToTable("list_send_lab");

                entity.Property(e => e.SendLabId).HasColumnName("send_lab_id");

                entity.Property(e => e.SendLabName)
                    .HasColumnType("character varying")
                    .HasColumnName("send_lab_name");
            });

            modelBuilder.Entity<TblIncomingBlood>(entity =>
            {
                entity.HasKey(e => e.BloodId)
                    .HasName("tbl_incoming_blood_pkey");

                entity.ToTable("tbl_incoming_blood");

                entity.Property(e => e.PatientId).HasColumnName("patient_id");

                entity.Property(e => e.BloodId).HasColumnName("blood_id");

                entity.Property(e => e.SendDistrictId).HasColumnName("send_district_id");

                entity.Property(e => e.SendLabId).HasColumnName("send_lab_id");

                entity.Property(e => e.CategoryPatientId).HasColumnName("category_patient_id");

                entity.Property(e => e.AnonymousPatient).HasColumnName("anonymous_patient");

                entity.Property(e => e.DateBloodSampling)
                    .HasColumnType("date")
                    .HasColumnName("d_blood_sampling");

                entity.Property(e => e.QualitySerumId).HasColumnName("quality_serum_id");

                entity.Property(e => e.DateBloodImport)
                    .HasColumnType("date")
                    .HasColumnName("d_blood_import");

                entity.Property(e => e.NumIfa).HasColumnName("num_ifa");

                entity.Property(e => e.NumInList).HasColumnName("num_in_list");

                entity.HasOne(d => d.CategoryPatientNavigation)
                    .WithMany(p => p.TblIncomingBloods)
                    .HasForeignKey(d => d.CategoryPatientId)
                    .HasConstraintName("fk_list_category");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.TblIncomingBloods)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("fk_tbl_patient_card");

                entity.HasOne(d => d.QualitySerumNavigation)
                    .WithMany(p => p.TblIncomingBloods)
                    .HasForeignKey(d => d.QualitySerumId)
                    .HasConstraintName("fk_list_quality_serum");

                entity.HasOne(d => d.SendDistrictNavigation)
                    .WithMany(p => p.TblIncomingBloods)
                    .HasForeignKey(d => d.SendDistrictId)
                    .HasConstraintName("fk_list_send_district");

                entity.HasOne(d => d.SendLabNavigation)
                    .WithMany(p => p.TblIncomingBloods)
                    .HasForeignKey(d => d.SendLabId)
                    .HasConstraintName("fk_list_send_lab");
            });

            modelBuilder.Entity<TblPatientCard>(entity =>
            {
                entity.HasKey(e => e.PatientId)
                    .HasName("tbl_patient_card_pkey");

                entity.ToTable("tbl_patient_card");

                entity.Property(e => e.PatientId).HasColumnName("patient_id");

                entity.Property(e => e.PatientCom)
                    .HasColumnType("character varying")
                    .HasColumnName("patient_com");

                entity.Property(e => e.AreaName)
                    .HasColumnType("character varying")
                    .HasColumnName("area_name");

                entity.Property(e => e.BirthDate)
                    .HasColumnType("date")    
                    .HasColumnName("birth_date");

                entity.Property(e => e.CityName)
                    .HasColumnType("character varying")
                    .HasColumnName("city_name");

                entity.Property(e => e.DateEdit)
                    .HasColumnType("date")
                    .HasColumnName("d_edit");

                entity.Property(e => e.FamilyName)
                    .HasMaxLength(30)
                    .HasColumnName("family_name");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(30)
                    .HasColumnName("first_name");

                entity.Property(e => e.PhoneNum)
                    .HasColumnType("character varying")
                    .HasColumnName("phone_num");

                entity.Property(e => e.RegionId).HasColumnName("region_id");

                entity.Property(e => e.SexId).HasColumnName("sex_id");

                entity.Property(e => e.ThirdName)
                    .HasMaxLength(30)
                    .HasColumnName("third_name");

                entity.Property(e => e.UserEdit).HasColumnName("user_edit");

                entity.Property(e => e.AddrHome)
                    .HasColumnType("character varying")
                    .HasColumnName("addr_home");

                entity.Property(e => e.AddrCorps)
                    .HasColumnType("character varying")
                    .HasColumnName("addr_corps");

                entity.Property(e => e.AddrFlat)
                    .HasColumnType("character varying")
                    .HasColumnName("addr_flat");

                entity.Property(e => e.AddrStreat)
                    .HasColumnType("character varying")
                    .HasColumnName("addr_streat");
                 
                entity.HasOne(d => d.Sex)
                    .WithMany(p => p.TblPatientCards)
                    .HasForeignKey(d => d.SexId)
                    .HasConstraintName("fk_sex");

                entity.HasOne(d => d.Region)
                    .WithMany(p => p.TblPatientCards)
                    .HasForeignKey(d => d.RegionId)
                    .HasConstraintName("fk_region");
            });

            modelBuilder.Entity<TblResultAntigen>(entity =>
            {
                entity.HasKey(e => e.ResultAntigenId)
                    .HasName("tbl_result_antigen_pkey");

                entity.ToTable("tbl_result_antigen");

                entity.Property(e => e.BloodId).HasColumnName("blood_id");

                entity.Property(e => e.ResultAntigenId).HasColumnName("result_antigen_id");

                entity.Property(e => e.ResultAntigenDate)
                    .HasColumnType("date")
                    .HasColumnName("d_result_antigen");

                entity.Property(e => e.ResultAntigenTestSysId).HasColumnName("result_antigen_test_system_id");

                entity.Property(e => e.ResultAntigenCutOff).HasColumnName("result_antigen_cut_off");

                entity.Property(e => e.ResultAntigenOp).HasColumnName("result_antigen_op");

                entity.Property(e => e.ResultAntigenConfirmOp).HasColumnName("result_antigen_confirm_op");

                entity.Property(e => e.ResultAntigenPercentGash).HasColumnName("result_antigen_percent_gash");

                entity.Property(e => e.ResultAntigenKp).HasColumnName("result_antigen_kp");

                entity.Property(e => e.ResultAntigenResultId).HasColumnName("result_antigen_result_id");

                entity.Property(e => e.ResultAntigenTypeId).HasColumnName("result_antigen_type_id");


                entity.HasOne(d => d.Blood)
                    .WithMany()
                    .HasForeignKey(d => d.BloodId)
                    .HasConstraintName("fk_tbl_incoming_blood");

                entity.HasOne(d => d.ResultAntigenResult)
                    .WithMany()
                    .HasForeignKey(d => d.ResultAntigenResultId)
                    .HasConstraintName("fk_list_result");

                entity.HasOne(d => d.TypeAntigen)
                    .WithMany()
                    .HasForeignKey(d => d.ResultAntigenTypeId)
                    .HasConstraintName("fk_type_antigen");

                entity.HasOne(d => d.TestSystem)
                    .WithMany()
                    .HasForeignKey(d => d.ResultAntigenTestSysId)
                    .HasConstraintName("fk_test_system");
            });

            modelBuilder.Entity<TblResultBlot>(entity =>
            {
                entity.HasKey(e => e.ResultBlotId)
                    .HasName("tbl_result_blot_pkey");

                entity.ToTable("tbl_result_blot");

                entity.Property(e => e.BloodId).HasColumnName("blood_id");

                entity.Property(e => e.ResultBlotId).HasColumnName("result_blot_id");

                entity.Property(e => e.ResultBlotDate)
                    .HasColumnType("date")
                    .HasColumnName("d_result_blot");

                entity.Property(e => e.ExpirationResultBlotDate)
                    .HasColumnType("date")
                    .HasColumnName("d_expiration_result_blot");

                entity.Property(e => e.ResultBlotTestSysId).HasColumnName("result_blot_test_sys_id");

                entity.Property(e => e.ResultBlotEnv160).HasColumnName("result_blot_env_160");

                entity.Property(e => e.ResultBlotEnv120).HasColumnName("result_blot_env_120");

                entity.Property(e => e.ResultBlotEnv41).HasColumnName("result_blot_env_41");

                entity.Property(e => e.ResultBlotGag55).HasColumnName("result_blot_gag_55");

                entity.Property(e => e.ResultBlotGag40).HasColumnName("result_blot_gag_40");

                entity.Property(e => e.ResultBlotGag2425).HasColumnName("result_blot_gag_24_25");

                entity.Property(e => e.ResultBlotGag18).HasColumnName("result_blot_gag_18");

                entity.Property(e => e.ResultBlotPol6866).HasColumnName("result_blot_pol_68_66");

                entity.Property(e => e.ResultBlotPol5251).HasColumnName("result_blot_pol_52_51");

                entity.Property(e => e.ResultBlotPol3431).HasColumnName("result_blot_pol_34_31");

                entity.Property(e => e.ResultBlotHiv2105).HasColumnName("result_blot_hiv2_105");

                entity.Property(e => e.ResultBlotHiv236).HasColumnName("result_blot_hiv2_36");

                entity.Property(e => e.ResultBlotHiv0).HasColumnName("result_blot_hiv0");

                entity.Property(e => e.ResultBlotReturnResult).HasColumnName("result_blot_return_result");

                entity.Property(e => e.ResultBlotResultId).HasColumnName("result_blot_result_id");


                entity.HasOne(d => d.Blood)
                    .WithMany()
                    .HasForeignKey(d => d.BloodId)
                    .HasConstraintName("fk_tbl_incoming_blood");

                entity.HasOne(d => d.ResultBlotResult)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotResultId)
                    .HasConstraintName("fk_result_blot");

                entity.HasOne(d => d.ResultBlotResultEnv120)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotEnv120)
                    .HasConstraintName("fk_result_env_120");

                entity.HasOne(d => d.ResultBlotResultEnv160)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotEnv160)
                    .HasConstraintName("fk_result_env_160");

                entity.HasOne(d => d.ResultBlotResultEnv41)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotEnv41)
                    .HasConstraintName("fk_result_env_41");

                entity.HasOne(d => d.ResultBlotResultGag18)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotGag18)
                    .HasConstraintName("fk_result_gag_18");

                entity.HasOne(d => d.ResultBlotResultGag2425)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotGag2425)
                    .HasConstraintName("fk_result_gag_24_25");

                entity.HasOne(d => d.ResultBlotResultGag40)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotGag40)
                    .HasConstraintName("fk_result_gag_40");

                entity.HasOne(d => d.ResultBlotResultGag55)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotGag55)
                    .HasConstraintName("fk_result_gag_55");

                entity.HasOne(d => d.ResultBlotResultHiv0)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotHiv0)
                    .HasConstraintName("fk_result_hiv0");

                entity.HasOne(d => d.ResultBlotResultHiv2105)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotHiv2105)
                    .HasConstraintName("fk_result_hiv2_105");

                entity.HasOne(d => d.ResultBlotResultHiv236)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotHiv236)
                    .HasConstraintName("fk_result_hiv2_36");

                entity.HasOne(d => d.ResultBlotResultPol3431)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotPol3431)
                    .HasConstraintName("fk_result_pol_34_31");

                entity.HasOne(d => d.ResultBlotResultPol5251)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotPol5251)
                    .HasConstraintName("fk_result_pol_52_51");

                entity.HasOne(d => d.ResultBlotResultPol6866)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotPol6866)
                    .HasConstraintName("fk_result_pol_68_66");

                entity.HasOne(d => d.TestSystem)
                    .WithMany()
                    .HasForeignKey(d => d.ResultBlotTestSysId)
                    .HasConstraintName("fk_test_system");
            });

            modelBuilder.Entity<TblResultIfa>(entity =>
            {
                entity.HasKey(e => e.ResultIfaId)
                    .HasName("tbl_result_ifa_pkey");

                entity.ToTable("tbl_result_ifa");

                entity.Property(e => e.BloodId).HasColumnName("blood_id");

                entity.Property(e => e.ResultIfaId).HasColumnName("result_ifa_id");

                entity.Property(e => e.ResultIfaDate)
                    .HasColumnType("date")
                    .HasColumnName("d_result_ifa");

                entity.Property(e => e.ResultIfaTestSysId).HasColumnName("result_ifa_test_sys_id");

                entity.Property(e => e.ResultIfaCutOff).HasColumnName("result_ifa_cut_off");

                entity.Property(e => e.ResultIfaOp).HasColumnName("result_ifa_op");

                entity.Property(e => e.ResultIfaKp).HasColumnName("result_ifa_kp");

                entity.Property(e => e.ResultIfaResultId).HasColumnName("result_ifa_result_id");


                entity.HasOne(d => d.Blood)
                    .WithMany(p => p.TblResultIfas)
                    .HasForeignKey(d => d.BloodId)
                    .HasConstraintName("fk_tbl_incoming_blood");

                entity.HasOne(d => d.ResultIfaResult)
                    .WithMany()
                    .HasForeignKey(d => d.ResultIfaResultId)
                    .HasConstraintName("fk_list_result");

                entity.HasOne(d => d.TestSystem)
                    .WithMany()
                    .HasForeignKey(d => d.ResultIfaTestSysId)
                    .HasConstraintName("fk_test_system");
            });

            modelBuilder.Entity<TblResultPcr>(entity =>
            {
                entity.HasKey(e => e.ResultPcrId)
                    .HasName("tbl_result_pcr_pkey");

                entity.ToTable("tbl_result_pcr");

                entity.Property(e => e.BloodId).HasColumnName("blood_id");

                entity.Property(e => e.ResultPcrId).HasColumnName("result_pcr_id");

                entity.Property(e => e.ResultPcrDate)
                    .HasColumnType("date")
                    .HasColumnName("d_result_pcr");

                entity.Property(e => e.ResultPcrTestSysId).HasColumnName("result_pcr_test_system_id");

                entity.Property(e => e.ResultPcrResultId).HasColumnName("result_pcr_result_id");

                entity.HasOne(d => d.Blood)
                    .WithMany()
                    .HasForeignKey(d => d.BloodId)
                    .HasConstraintName("fk_tbl_incoming_blood");

                entity.HasOne(d => d.ResultPcrResult)
                    .WithMany()
                    .HasForeignKey(d => d.ResultPcrResultId)
                    .HasConstraintName("fk_list_result");

                entity.HasOne(d => d.TestSystem)
                    .WithMany()
                    .HasForeignKey(d => d.ResultPcrTestSysId)
                    .HasConstraintName("fk_test_system");
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tbl_users");

                entity.Property(e => e.UserFio)
                    .HasMaxLength(30)
                    .HasColumnName("user_fio");

                entity.Property(e => e.UserId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("user_id");

                entity.Property(e => e.UserPassword)
                    .HasColumnType("character varying")
                    .HasColumnName("user_password");

                entity.Property(e => e.UserPosition)
                    .HasMaxLength(30)
                    .HasColumnName("user_position");
            });

            modelBuilder.Entity<ListSex>(entity =>
            {
                entity.HasKey(e => e.SexId)
                    .HasName("list_sex_pkey");

                entity.ToTable("list_sex");

                entity.Property(e => e.SexId).HasColumnName("sex_id");

                entity.Property(e => e.SexNameLong)
                    .HasColumnType("character varying")
                    .HasColumnName("sex_name_long");

                entity.Property(e => e.SexNameShort)
                    .HasColumnType("character varying")
                    .HasColumnName("sex_name_short");
            });

            modelBuilder.Entity<TblDistrictBlot>(entity =>
            {
                entity.HasKey(e => e.DistrictBlotId)
                    .HasName("tbl_district_blot_pkey");

                entity.ToTable("tbl_district_blot");

                entity.Property(e => e.PatientId).HasColumnName("patient_id");

                entity.Property(e => e.DBlot)
                    .HasColumnType("date")
                    .HasColumnName("d_blot");

                entity.Property(e => e.CutOff).HasColumnName("cut_off");

                entity.Property(e => e.BlotResult).HasColumnName("blot_result");

                entity.Property(e => e.BlotCoefficient).HasColumnName("blot_coefficient");

                entity.Property(e => e.TestSystemId).HasColumnName("test_system_id");

                entity.Property(e => e.SendDistrictId).HasColumnName("send_district_id");

                entity.Property(e => e.DistrictBlotId).HasColumnName("district_blot_id");

                entity.Property(e => e.SendLabId).HasColumnName("send_lab_id");


                entity.HasOne(d => d.Patient)
                    .WithMany()
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("fk_patient_id");

                entity.HasOne(d => d.SendDistrict)
                    .WithMany()
                    .HasForeignKey(d => d.SendDistrictId)
                    .HasConstraintName("fk_send_district");

                entity.HasOne(d => d.TestSystem)
                    .WithMany()
                    .HasForeignKey(d => d.TestSystemId)
                    .HasConstraintName("fk_test_system");

                entity.HasOne(d => d.SendLab)
                    .WithMany()
                    .HasForeignKey(d => d.SendLabId)
                    .HasConstraintName("fk_send_lab");

            });

            modelBuilder.Entity<ListRegion>(entity =>
            {
                entity.HasKey(e => e.RegionId)
                    .HasName("list_region_pkey");

                entity.ToTable("list_region");

                entity.Property(e => e.RegionId).HasColumnName("region_id");

                entity.Property(e => e.RegionName)
                    .HasColumnType("character varying")
                    .HasColumnName("region_name");

                entity.Property(e => e.RegionType)
                    .HasColumnName("region_type");
            });

            modelBuilder.Entity<ListTestSystem>(entity =>
            {
                entity.HasKey(e => e.TestSystemId)
                    .HasName("list_test_system_pkey");

                entity.ToTable("list_test_system");

                entity.Property(e => e.TestSystemId).HasColumnName("test_system_id");

                entity.Property(e => e.TestSystemName)
                    .HasColumnType("character varying")
                    .HasColumnName("test_system_name");

                entity.Property(e => e.TestSystemSeries)
                    .HasColumnType("character varying")
                    .HasColumnName("test_system_series");

                entity.Property(e => e.DTestSystemShelfLife).HasColumnName("d_test_system_shelf_life");

            });

            modelBuilder.Entity<ListTypeAntigen>(entity =>
            {
                entity.HasKey(e => e.TypeAntigenId)
                    .HasName("list_type_antigen_pkey");

                entity.ToTable("list_type_antigen");

                entity.Property(e => e.TypeAntigenId).HasColumnName("type_antigen_id");

                entity.Property(e => e.TypeAntigenName)
                    .HasColumnType("character varying")
                    .HasColumnName("type_antigen_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
