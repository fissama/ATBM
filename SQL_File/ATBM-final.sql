ALTER SESSION SET "_oracle_script"=TRUE; 
CREATE USER BV IDENTIFIED BY 1;
GRANT DBA TO BV;
GRANT CREATE SESSION TO BV;
-------------------TAO BANG (BAT DAU CHAY CODE TU DAY TOI CUOI)

ALTER TABLE PHIEU_KHAM DROP CONSTRAINT fk_pk_bn;
ALTER TABLE PHIEU_KHAM DROP CONSTRAINT fk_pk_nv;

ALTER TABLE PK_DV DROP CONSTRAINT fk_pkdv_pk;
ALTER TABLE PK_DV DROP CONSTRAINT fk_pkdv_dv;

ALTER TABLE PK_THUOC DROP CONSTRAINT fk_pkth_pk;
ALTER TABLE PK_THUOC DROP CONSTRAINT fk_pkth_th;

ALTER TABLE NHAN_VIEN DROP CONSTRAINT fk_nv_lnv;
ALTER TABLE NHAN_VIEN DROP CONSTRAINT fk_nv_pb;

ALTER TABLE CHAM_CONG DROP CONSTRAINT fk_cc_nv;

ALTER TABLE PHONG_BAN DROP CONSTRAINT fk_pb_bp;
ALTER TABLE PHONG_BAN DROP CONSTRAINT fk_pb_nv;
BEGIN
    EXECUTE IMMEDIATE 'DROP TABLE BENH_NHAN';
    EXECUTE IMMEDIATE 'DROP TABLE BO_PHAN';
    EXECUTE IMMEDIATE 'DROP TABLE CHAM_CONG';
    EXECUTE IMMEDIATE 'DROP TABLE DICH_VU';
    EXECUTE IMMEDIATE 'DROP TABLE LOAI_NV';
    EXECUTE IMMEDIATE 'DROP TABLE NHAN_VIEN';
    EXECUTE IMMEDIATE 'DROP TABLE PHIEU_KHAM';
    EXECUTE IMMEDIATE 'DROP TABLE PHONG_BAN';
    EXECUTE IMMEDIATE 'DROP TABLE PK_DV';
    EXECUTE IMMEDIATE 'DROP TABLE PK_THUOC';
    EXECUTE IMMEDIATE 'DROP TABLE THUOC';
EXCEPTION
    WHEN OTHERS THEN
    NULL;
END;
/

-------------------TAO BANG (BAT DAU CHAY CODE TU DAY TOI CUOI)
CREATE TABLE BV.BENH_NHAN(--DONE
    MaBN number,
    TenBN nvarchar2(30),
    Phai nvarchar2(5),
    DiaChi nvarchar2(50),
    SDT varchar2(10),
    CMND varchar2(15),
    NgaySinh varchar(15),
    CONSTRAINT pk_mabn PRIMARY KEY(MaBN)
);

CREATE TABLE BV.PHIEU_KHAM(--DONE
    MaPK number,
    MaBN number,
    MaNV number,
    NgayKham date,
    TrieuChung nvarchar2(20),
    TongTien float,
    TinhTrang int DEFAULT 1, --Qua phong tai vu: 'Xong tien kham '~2 -> 
                                            --a. Qua phong bac si, bac si nhan nut THÊM THUÔC:  -> 'Thu tien Thuoc'~5 --Bán thuoc se tim nhung PK co tinh trang =5
                                            --                           b. else THÊM DICH VU -> 'THU DV' ~3
                                                                                    -- Tai Vu thay nhung phieu khám có DV ~ 3 thì tính tien
                                                                                    --                          sau khi tính xong thi Tinh Trang = 'Xong tien DV' ~4
                                                                                  --Quay lai phong Bác si kê thuoc và nhap thuoc cho benh nhan 'Thu tien thuoc' ~5
                                            --Nhung nguoi ban thuoc thi thay 'Thu thuoc' ~5
    --1: Thu tien khám
    --2: Xong tien khám
    --3: Tien DV
    --4: Xong tien DV
    --5: Tien thuoc
    --0: Hoan thanh
    CONSTRAINT pk_mapk PRIMARY KEY (MaPK)
 );
  
CREATE TABLE BV.PK_DV(--DONE
   MaPK number,
   MaDV number,
    CONSTRAINT pk_mapk_madv PRIMARY KEY (MaPK,MaDV)
);

CREATE TABLE BV.DICH_VU(--DONE
    MaDV number,
    TenDV nvarchar2(30),
    Gia float,
    CONSTRAINT pk_madv PRIMARY KEY (MaDV)
);

CREATE TABLE BV.PK_THUOC(--DONE
    MaPK number,
    MaThuoc number,
    SoLuong number,
    CONSTRAINT pk_mapk_mathuoc PRIMARY KEY (MaPK, MaThuoc)
);

CREATE TABLE BV.THUOC (--DONE
    MaThuoc number,
    TenThuoc nvarchar2(20),
    LoaiThuoc nvarchar2(20),
    GiaTien float,
    CONSTRAINT pk_mathuoc PRIMARY KEY (MaThuoc)
);

CREATE TABLE BV.NHAN_VIEN( --DONE
    MaNV number,
    TenNV nvarchar2(31),
    LuongNV varchar2(200),
    LoaiNV nvarchar2(20),
    MaPB number,
    USERNAME VARCHAR2(20),
    CONSTRAINT pk_manv PRIMARY KEY (MaNV)
);

CREATE TABLE BV.BO_PHAN(--DONE
    MaBP number,
    TenBP nvarchar2(31),
    CONSTRAINT pk_mabp PRIMARY KEY (MaBP)
);
CREATE TABLE BV.LOAI_NV ( --DONE
    MaLoai nvarchar2(20),
    TenLoai nvarchar2(20),
    CONSTRAINT pk_maloai PRIMARY KEY (MaLoai)   
);

CREATE TABLE BV.PHONG_BAN(--DONE
    MaPB number,
    TenPB nvarchar2(50),
    MaBP number,
    MaTrPh number,
    CONSTRAINT pk_mapb PRIMARY KEY (MaPB)
);


CREATE TABLE BV.CHAM_CONG(-- DONE
    MaNV number,
    NgayLam date,
    TuGio varchar2(10),
    DenGio varchar2(10),
    CONSTRAINT pk_manv_ngaylam PRIMARY KEY (MaNV, NgayLam)
);

-------TAO KHOA NGOAI
--PHIEU_KHAM--DONE
ALTER TABLE BV.PHIEU_KHAM
ADD CONSTRAINT fk_pk_bn FOREIGN KEY (MaBN) REFERENCES BV.BENH_NHAN(MaBN);

ALTER TABLE BV.PHIEU_KHAM
ADD CONSTRAINT fk_pk_nv FOREIGN KEY (MaNV) REFERENCES BV.NHAN_VIEN(MaNV);

--PK_DV--DONE
ALTER TABLE BV.PK_DV
ADD CONSTRAINT fk_pkdv_pk FOREIGN KEY (MaPK) REFERENCES BV.PHIEU_KHAM(MaPK);

ALTER TABLE BV.PK_DV
ADD CONSTRAINT fk_pkdv_dv FOREIGN KEY (MaDV) REFERENCES BV.DICH_VU(MaDV);

--PK_THUOC--DONE
ALTER TABLE BV.PK_THUOC
ADD CONSTRAINT fk_pkth_pk FOREIGN KEY (MaPK) REFERENCES BV.PHIEU_KHAM(MaPK);

ALTER TABLE BV.PK_THUOC
ADD CONSTRAINT fk_pkth_th FOREIGN KEY (MaThuoc) REFERENCES BV.THUOC(MaThuoc);

--NV--DONE
ALTER TABLE BV.NHAN_VIEN
ADD CONSTRAINT fk_nv_lnv FOREIGN KEY (LoaiNV) REFERENCES BV.Loai_NV(MaLoai);

ALTER TABLE BV.NHAN_VIEN
ADD CONSTRAINT fk_nv_pb FOREIGN KEY (MaPB) REFERENCES BV.PHONG_BAN(MaPB);

--CHAMCONG--DONE

ALTER TABLE BV.CHAM_CONG
ADD CONSTRAINT fk_cc_nv FOREIGN KEY (MaNV) REFERENCES BV.NHAN_VIEN(MaNV);

--PHONGBAN--DONE
ALTER TABLE BV.PHONG_BAN
ADD CONSTRAINT fk_pb_bp FOREIGN KEY (MaBP) REFERENCES BV.BO_PHAN(MaBP);

ALTER TABLE BV.PHONG_BAN
ADD CONSTRAINT fk_pb_nv FOREIGN KEY (MaTrPh) REFERENCES BV.NHAN_VIEN(MaNV);

/
-----Tao function ma hoa luong
CREATE OR REPLACE FUNCTION En_Luong(luong IN number)
return varchar2
is 
    key_bytes_raw RAW (256) := UTL_RAW.CAST_TO_RAW('ATBMATBMATBMATBMATBMATBMATBMATBM');
    encrypted_raw RAW (100);
    encryption_type number := DBMS_CRYPTO.ENCRYPT_AES256 + DBMS_CRYPTO.CHAIN_CBC + DBMS_CRYPTO.PAD_PKCS5;
begin
    encrypted_raw := DBMS_CRYPTO.ENCRYPT(
        src =>  UTL_RAW.CAST_TO_RAW(luong),
        typ =>  encryption_type,
        key => key_bytes_raw);
    return UTL_RAW.CAST_TO_VARCHAR2(encrypted_raw);
end;
/

--Tao function giai ma luong
CREATE OR REPLACE FUNCTION De_Luong(luong IN varchar2)
return number
is 
    key_bytes_raw RAW (256) := UTL_RAW.CAST_TO_RAW('ATBMATBMATBMATBMATBMATBMATBMATBM');
    decrypted_raw RAW (100);
    decryption_type number := DBMS_CRYPTO.ENCRYPT_AES256 + DBMS_CRYPTO.CHAIN_CBC + DBMS_CRYPTO.PAD_PKCS5;
begin
    decrypted_raw := DBMS_CRYPTO.DECRYPT(
        src =>  UTL_RAW.CAST_TO_RAW(luong),
        typ =>  decryption_type,
        key =>  key_bytes_raw
);
    return  TO_NUMBER(UTL_RAW.CAST_TO_VARCHAR2(decrypted_raw));
end;
/

--Them vao cac dong du lieu
INSERT INTO BV.BENH_NHAN(MaBN,TenBN,Phai,DiaChi,SDT,CMND,NgaySinh) values(1,N'Dong Hung','Nam',N'10 Tran Binh Trong Q5','0123456789','123456789', date '1999-08-15');
INSERT INTO BV.BENH_NHAN(MaBN,TenBN,Phai,DiaChi,SDT,CMND,NgaySinh) values(2,N'Tuyet Nhi','Nu',N'157 Cao Thang Q3','0011223344','135792468',date '1999-01-13');
INSERT INTO BV.BENH_NHAN(MaBN,TenBN,Phai,DiaChi,SDT,CMND,NgaySinh) values(3,N'Thien Tin','Nam',N'8 Ben Thanh Q1','0135792468','012345673',date '1999-03-26');
INSERT INTO BV.BENH_NHAN(MaBN,TenBN,Phai,DiaChi,SDT,CMND,NgaySinh) values(4,N'Thong Le','Nam',N'24 Su Van Han Q10','0987654321','234567892',date '1999-07-30');
INSERT INTO BV.BENH_NHAN(MaBN,TenBN,Phai,DiaChi,SDT,CMND,NgaySinh) values(5,N'Nha Trang','Nu',N'6 An Binh Q5','0777777777','031232424',date '1999-12-31');

INSERT INTO BV.DICH_VU(MaDV,TenDV,Gia) values(1,'Xet nghiem mau',500000 );
INSERT INTO BV.DICH_VU(MaDV,TenDV,Gia) values(2,'Kham tong quat',1500000 );
INSERT INTO BV.DICH_VU(MaDV,TenDV,Gia) values(3,'Tieu Phau',1000000 );
INSERT INTO BV.DICH_VU(MaDV,TenDV,Gia) values(4,'Tiem vacxin',500000 );
INSERT INTO BV.DICH_VU(MaDV,TenDV,Gia) values(5,'Xet nghiem nuoc tieu',2500000 );
INSERT INTO BV.DICH_VU(MaDV,TenDV,Gia) values(6,'Kham tong quat',150000 );

INSERT INTO BV.BO_PHAN(MaBP,TenBP) values(1,'Bo phan quan ly');
INSERT INTO BV.BO_PHAN(MaBP,TenBP) values(2,'Bo phan tiep tan,dieu phoi');
INSERT INTO BV.BO_PHAN(MaBP,TenBP) values(3,'Bo phan ban thuoc');
INSERT INTO BV.BO_PHAN(MaBP,TenBP) values(4,'Bo phan quan tri cong ty');
INSERT INTO BV.BO_PHAN(MaBP,TenBP) values(5,'Bo phan ke toan');
INSERT INTO BV.BO_PHAN(MaBP,TenBP) values(6,'Bo phan kham benh');


INSERT INTO BV.PHONG_BAN(MaPB,MaBP,TenPB) values(1,4,'Phong giam doc');
INSERT INTO BV.PHONG_BAN(MaPB,MaBP,TenPB) values(2,1,'Phong tai vu');
INSERT INTO BV.PHONG_BAN(MaPB,MaBP,TenPB) values(3,5,'Phong ke toan');
INSERT INTO BV.PHONG_BAN(MaPB,MaBP,TenPB) values(4,1,'Phong tai nguyen va nhan su');
INSERT INTO BV.PHONG_BAN(MaPB,MaBP,TenPB) values(5,1,'Phong quan ly chuyen mon');
INSERT INTO BV.PHONG_BAN(MaPB,MaBP,TenPB) values(6,2,'Phong tiep tan, dieu phoi');
INSERT INTO BV.PHONG_BAN(MaPB,MaBP,TenPB) values(7,3,'Phong ban thuoc');
INSERT INTO BV.PHONG_BAN(MaPB,MaBP,TenPB) values(8,6,'Phong bac si va y ta');

INSERT INTO BV.LOAI_NV(MaLoai,TenLoai) values('001','Bac si');
INSERT INTO BV.LOAI_NV(MaLoai,TenLoai) values('002','Y ta');
INSERT INTO BV.LOAI_NV(MaLoai,TenLoai) values('003','Ban thuoc');
INSERT INTO BV.LOAI_NV(MaLoai,TenLoai) values('004','Tiep tan');
INSERT INTO BV.LOAI_NV(MaLoai,TenLoai) values('005','Giam doc');
INSERT INTO BV.LOAI_NV(MaLoai,TenLoai) values('006','NV van phong');


--INSERT INTO BV.NHAN_VIEN(MaNV,TenNV,LuongNV,LoaiNV,MaPB) VALUES(1,'Tuan Khang','50000000','005',1);
INSERT INTO BV.NHAN_VIEN(MaNV,TenNV,LuongNV,LoaiNV,MaPB,USERNAME) VALUES(2,'Quoc Khanh',BV.en_luong('10000'),'001',8,'BS1');
INSERT INTO BV.NHAN_VIEN(MaNV,TenNV,LuongNV,LoaiNV,MaPB,USERNAME) VALUES(3,'Hoai Phat',BV.en_luong('500000'),'001',8,'BS2');
INSERT INTO BV.NHAN_VIEN(MaNV,TenNV,LuongNV,LoaiNV,MaPB,USERNAME) VALUES(4,'Anh Phi',BV.en_luong('7000000'),'004',6,'TT1');
INSERT INTO BV.NHAN_VIEN(MaNV,TenNV,LuongNV,LoaiNV,MaPB,USERNAME) VALUES(5,'Phuc Khang',BV.en_luong('20000'),'003',7,'BT1');
INSERT INTO BV.NHAN_VIEN(MaNV,TenNV,LuongNV,LoaiNV,MaPB,USERNAME) VALUES(6,'Anh Quan',BV.en_luong('10000'),'006',4,'NS1');
--INSERT INTO BV.NHAN_VIEN(MaNV,TenNV,LuongNV,LoaiNV,MaPB) VALUES(7,'Tu Dat',BV.en_luong'10000','006',5);
INSERT INTO BV.NHAN_VIEN(MaNV,TenNV,LuongNV,LoaiNV,MaPB) VALUES(8,'Minh Tuan',BV.en_luong('10000'),'006',2);
INSERT INTO BV.NHAN_VIEN(MaNV,TenNV,LuongNV,LoaiNV,MaPB,USERNAME) VALUES(9,'Thien Kim',BV.en_luong('10000'),'006',3,'KT1');
INSERT INTO BV.NHAN_VIEN(MaNV,TenNV,LuongNV,LoaiNV,MaPB,USERNAME) VALUES(10,'Truc Linh',BV.en_luong('10000'),'006',2,'TPTaiVu');
INSERT INTO BV.NHAN_VIEN(MaNV,TenNV,LuongNV,LoaiNV,MaPB,USERNAME) VALUES(11,'Thi Hon',BV.en_luong('10000'),'006',3,'TPKeToan');
INSERT INTO BV.NHAN_VIEN(MaNV,TenNV,LuongNV,LoaiNV,MaPB,USERNAME) VALUES(12,'Thanh Hong',BV.en_luong('10000'),'006',4,'TPNhanSu');
INSERT INTO BV.NHAN_VIEN(MaNV,TenNV,LuongNV,LoaiNV,MaPB,USERNAME) VALUES(13,'Tran Duy',BV.en_luong('10000'),'006',5,'TPChuyenMon');

UPDATE BV.PHONG_BAN SET MaTrPh = 10 Where MaPB=2;
UPDATE BV.PHONG_BAN SET MaTrPh = 11 Where MaPB=3;
UPDATE BV.PHONG_BAN SET MaTrPh = 12 Where MaPB=4;
UPDATE BV.PHONG_BAN SET MaTrPh = 13 Where MaPB=5;


INSERT INTO BV.CHAM_CONG(MaNV,NgayLam,TuGio,DenGio) values(2, date '2020-08-28','8:00 AM','17:00 PM');
INSERT INTO BV.CHAM_CONG(MaNV,NgayLam,TuGio,DenGio) values(3, date '2020-08-28','8:00 AM','17:00 PM');
INSERT INTO BV.CHAM_CONG(MaNV,NgayLam,TuGio,DenGio) values(4, date '2020-08-28','8:00 AM','17:00 PM');
INSERT INTO BV.CHAM_CONG(MaNV,NgayLam,TuGio,DenGio) values(4, date '2020-08-27','8:00 AM','17:00 PM');
INSERT INTO BV.CHAM_CONG(MaNV,NgayLam,TuGio,DenGio) values(4, date '2020-08-26','8:00 AM','17:00 PM');
INSERT INTO BV.CHAM_CONG(MaNV,NgayLam,TuGio,DenGio) values(4, date '2020-08-25','8:00 AM','17:00 PM');
INSERT INTO BV.CHAM_CONG(MaNV,NgayLam,TuGio,DenGio) values(5, date '2020-08-28','8:00 AM','17:00 PM');
INSERT INTO BV.CHAM_CONG(MaNV,NgayLam,TuGio,DenGio) values(6, date '2020-08-28','8:00 AM','17:00 PM');
INSERT INTO BV.CHAM_CONG(MaNV,NgayLam,TuGio,DenGio) values(9, date '2020-08-28','8:00 AM','17:00 PM');
INSERT INTO BV.CHAM_CONG(MaNV,NgayLam,TuGio,DenGio) values(10, date '2020-08-28','8:00 AM','17:00 PM');
INSERT INTO BV.CHAM_CONG(MaNV,NgayLam,TuGio,DenGio) values(11, date '2020-08-28','8:00 AM','17:00 PM');
INSERT INTO BV.CHAM_CONG(MaNV,NgayLam,TuGio,DenGio) values(12, date '2020-08-28','8:00 AM','17:00 PM');

INSERT INTO BV.Thuoc(MaThuoc,TenThuoc,LoaiThuoc,GiaTien) values(1,'Avastin','Dieu tri',111);
INSERT INTO BV.Thuoc(MaThuoc,TenThuoc,LoaiThuoc,GiaTien) values(2,'Bonia','Tiem',111);
INSERT INTO BV.Thuoc(MaThuoc,TenThuoc,LoaiThuoc,GiaTien) values(3,'Adrenalin','Thuoc bo',222);
INSERT INTO BV.Thuoc(MaThuoc,TenThuoc,LoaiThuoc,GiaTien) values(4,'flagyl','Khang sinh',111);
INSERT INTO BV.Thuoc(MaThuoc,TenThuoc,LoaiThuoc,GiaTien) values(5,'Jamda','Dieu tri',333);

INSERT INTO BV.PHIEU_KHAM(MaPK,MaBN,MaNV,NgayKham,TrieuChung,TongTien) values(1,1,2,date '2020-09-01','Ho',230000);
INSERT INTO BV.PHIEU_KHAM(MaPK,MaBN,MaNV,NgayKham,TrieuChung,TongTien) values(2,2,3,date '2020-09-02','Sot',330000);
INSERT INTO BV.PHIEU_KHAM(MaPK,MaBN,MaNV,NgayKham,TrieuChung,TongTien) values(3,3,3,date '2020-09-03','Dau chan',130000);
INSERT INTO BV.PHIEU_KHAM(MaPK,MaBN,MaNV,NgayKham,TrieuChung,TongTien) values(4,4,2,date'2020-09-04','Gay xuong',430000);
INSERT INTO BV.PHIEU_KHAM(MaPK,MaBN,MaNV,NgayKham,TrieuChung,TongTien) values(5,5,2,date '2020-09-05','Trat khop',630000);

INSERT INTO BV.PK_DV(MaDV,MaPK) values(1,3);
INSERT INTO BV.PK_DV(MaDV,MaPK) values(2,4);
INSERT INTO BV.PK_DV(MaDV,MaPK) values(3,1);
INSERT INTO BV.PK_DV(MaDV,MaPK) values(4,2);
INSERT INTO BV.PK_DV(MaDV,MaPK) values(5,5);

INSERT INTO BV.PK_THUOC(MaPK,MaThuoc,SoLuong) values(1,1,2);
INSERT INTO BV.PK_THUOC(MaPK,MaThuoc,SoLuong) values(2,2,3);
INSERT INTO BV.PK_THUOC(MaPK,MaThuoc,SoLuong) values(3,3,4);
INSERT INTO BV.PK_THUOC(MaPK,MaThuoc,SoLuong) values(4,4,5);
INSERT INTO BV.PK_THUOC(MaPK,MaThuoc,SoLuong) values(5,5,1);


-----------------------------------------------------------------RBAC TIEP TAN--------------------------------------------------
ALTER SESSION SET "_oracle_script"=TRUE; 

--Tao role vao user
create role TiepTan;
create user TT1 identified by 1;
grant create session to TT1;
grant TiepTan to TT1;
/
--Cho phep tiep tan chinh sua tren View BENH_NHAN (tao view -> gan quyen tren view -> tao instead of trigger)
--grant select, insert, update, delete on BENH_NHAN to TiepTan; 
create or replace view INFO_BENH_NHAN_TT as
    select MaBN, TenBN, Phai, DiaChi, Sdt, Cmnd, NgaySinh
    from BV.BENH_NHAN;
/
grant select, insert, update on BV.INFO_BENH_NHAN_TT to TiepTan;
/
create or replace trigger Insert_BN_View_Trigger
instead of insert on BV.INFO_BENH_NHAN_TT
begin
    Insert into BV.BENH_NHAN(MaBN, TenBN, Phai, DiaChi, Sdt, Cmnd, NgaySinh) Values (:new.MaBN, :new.TenBN, :new.Phai, :new.DiaChi, :new.Sdt, :new.Cmnd, :new.NgaySinh);
end;
/
create or replace trigger Update_BN_View_Trigger
instead of update on BV.INFO_BENH_NHAN_TT
begin
    Update BV.BENH_NHAN
    set MaBN = :new.MaBN, TenBN = :new.TenBN, Phai = :new.Phai, DiaChi = :new.DiaChi, Sdt =  :new.Sdt, NgaySinh = :new.NgaySinh
    where Cmnd=:old.Cmnd;
end;
/

--Cho phep tiep tan tao PHIEU KHAM moi tren View cho benh nhan voi thong tin co ban (tao view -> gan quyen tren view -> tao instead of trigger)

create or replace view PHIEU_KHAM_TT as
    select pk.MaPK, bn.MaBN, bn.TenBN , nv.MaNV, nv.TenNV, pk.NgayKham
    from PHIEU_KHAM pk, BENH_NHAN bn, NHAN_VIEN nv
    where pk.MaBN = bn.MaBN and pk.MaNV=nv.MaNV;
/    
grant select on BV.PHIEU_KHAM_TT to TiepTan;
/
create or replace trigger Insert_PK_View_Trigger
instead of insert on BV.PHIEU_KHAM_TT
begin
    Insert into BV.PHIEU_KHAM(MaPK, MaBN, MaNV, NgayKham) Values (:new.MaPK, :new.MaBN, :new.MaNV, :new.NgayKham);
end;
/
--Tao view cho role tiep tan 
    
create or replace view INFO_BSI_TT as
    select MaNV, TenNV from NHAN_VIEN
    where LoaiNV=001;
/
    
 --Cap quyen view cho tiep tan   

-- grant select on PK_THUOC_TT to TiepTan;
 grant select on INFO_BSI_TT to TiepTan;
 /

 --Tao phieu kham cho benh nhan
 create or replace procedure TAO_PK_TT (mpk number,nv number)
 as
    x number;
begin 
    select max(MaBN) into x
    from BV.INFO_BENH_NHAN_TT;
    
    insert into BV.PHIEU_KHAM_TT(MaPK, MaBN, MaNV, NgayKham) values (mpk, x, nv, current_date);
end;
/
--Them moi benh nhan
create or replace procedure THEM_BN_TT (ten nvarchar2, gtinh nvarchar2, dchi nvarchar2, sdthoai varchar2, id_cmnd varchar2, dob varchar2)
as
    x number;
begin
    select max(MaBN)+1 into x
    from BV.INFO_BENH_NHAN_TT;
    
    insert into BV.INFO_BENH_NHAN_TT(MaBN, TenBN, Phai, DiaChi, Sdt, Cmnd, NgaySinh) values (x, ten, gtinh, dchi, sdthoai, id_cmnd, dob);
end;
/

--Gan quyen cho role Tiep Tan nhung procedure tren
Grant execute on BV.TAO_PK_TT to TiepTan;
Grant execute on BV.THEM_BN_TT to TiepTan;
/

-----------------------------DAC TAI VU--------------------------------------DAC TAI VU-------------------------DAC TAI VU----------------------------------------------
create user TV1 identified by 1;
grant create session to TV1;

/
-------Tao View de thu tien nhung phieu kham 
create or replace view PK_TV as
    select pk.MaPK, bn.TenBN, pk.TongTien, pk.TinhTrang
    from PHIEU_KHAM pk, BENH_NHAN bn
    where pk.MaBN=bn.MaBN;
/    
--Gan quyen tren View
grant select, update(TongTien, TinhTrang) on BV.PK_TV to TV1;
/
--Trigger cap nhat tien kham benh va tinh trang phieu kham tu View qua Bang chinh
create or replace trigger Update_PK_TienKham_View_Trigger
instead of update on BV.PK_TV
begin
    Update BV.PHIEU_KHAM
    set TongTien = :new.TongTien, TinhTrang=:new.TinhTrang
    where MaPK=:old.MaPK;
end;
/
--Tao View cho tai vu xem cac Dich vu
create or replace view DICH_VU_TV as    
    select MaDV, TenDV, Gia
    from DICH_VU;
 /   
--Gan quyen View
grant select, update on BV.DICH_VU_TV to TV1;
/
--Trigger cap nhan gia tien dich vu tu view sang bang chinh
create or replace trigger Update_DV_Tien_View_Trigger
instead of update on BV.DICH_VU_TV
begin
    Update BV.DICH_VU
    set Gia = :new.Gia
    where MaDV=:old.MaDV;
end;
/
--Tao view phieu kham dich vu cua benh nhan
create or replace view PKDV_BN_TV as
    select pkdv.MaPK, dv.MaDV, dv.TenDV, dv.Gia
    from PK_DV pkdv, DICH_VU dv
    where pkdv.MaDV= dv.MaDV;
/    
--Gan quyen tren view
grant select on BV.PKDV_BN_TV to TV1;

/
-------Tao view Thuoc cho Tai vu xem va dieu chinh gia
create or replace view THUOC_TV as
    select MaThuoc, TenThuoc, LoaiThuoc, GiaTien
    from BV.THUOC
/
--Gan quyen tren view cho Tai vu
grant select, update(GiaTien) on BV.THUOC_TV to TV1;
/
--Trigger update gia tien thuoc tu View sang Bang
create or replace trigger Update_Thuoc_Tien_View_Trigger
instead of update on BV.THUOC_TV
begin
    Update BV.THUOC
    set GiaTien = :new.GiaTien
    where MaThuoc=:old.MaThuoc;
end;
/
-------------BAC SI---------BAC SI---------BAC SI---------------BAC SI-----------BAC SI-------------BAC SI

--Tao user bac si va cap quyen tren cac bang lien quan
create user BS1 identified by 1;
grant create session to BS1;

create user BS2 identified by 1;
grant create session to BS2;

grant select on BV.BENH_NHAN to BS1;
grant select on BV.BENH_NHAN to BS2;

grant select, insert, update on BV.PHIEU_KHAM to BS1;
grant select, insert, update on BV.PHIEU_KHAM to BS2;

grant select on BV.DICH_VU to BS1;
grant select on BV.DICH_VU to BS2;

grant select, insert, update on BV.PK_DV to BS1;
grant select, insert, update on BV.PK_DV to BS2;

grant select, insert, update on BV.PK_THUOC to BS1;
grant select, insert, update on BV.PK_THUOC to BS2;

grant select on BV.THUOC to BS1;
grant select on BV.THUOC to BS2;
/
--Cai dat chinh sach VPD, bac si chi xem duoc nhung benh nhan do chinh minh kham
CREATE OR REPLACE FUNCTION BV.POLICY_BAC_SI_XEM_BENH_NHAN(p_schema IN VARCHAR2, p_object IN VARCHAR2)
RETURN VARCHAR2
AS
    CUR_USER VARCHAR2(20):=SYS_CONTEXT ('USERENV', 'SESSION_USER');
    
BEGIN 
    IF (SUBSTR(CUR_USER,1,2) = 'BS') THEN
        RETURN 'MaNV = (SELECT MaNV FROM BV.NHAN_VIEN WHERE USERNAME = '''|| CUR_USER ||''')';
    ELSE
        RETURN '';
    END IF;
END;
/
--Ap dung chinh sach vao bang Phieu Kham
BEGIN
DBMS_RLS.ADD_POLICY(
OBJECT_SCHEMA => 'BV',
OBJECT_NAME => 'PHIEU_KHAM',
POLICY_NAME => 'POL_BV',
FUNCTION_SCHEMA => 'BV',
POLICY_FUNCTION => 'POLICY_BAC_SI_XEM_BENH_NHAN',
SEC_RELEVANT_COLS => 'MaPK, MaBN, MaNV, NgayKham, TrieuChung, TongTien, TinhTrang',
SEC_RELEVANT_COLS_OPT => DBMS_RLS.ALL_ROWS,
UPDATE_CHECK => true
);
END;
/

------------------NV KE TOAN-----------------------NV KE TOAN-------------------NV KE TOAN-------------------NV KE TOAN---------------------------


create user KT1 identified by 1;
grant create session to KT1;
/
--Giai ma thong tin luong
grant execute on sys.dbms_crypto to KT1;
grant execute on BV.De_Luong to KT1;
grant execute on BV.En_Luong to KT1;

/
---Xem thong tin Luong cua nhan vien
create or replace View NHAN_VIEN_KT as
    select nv.MaNV, nv.TenNV, nv.LuongNV, (count(cc.NgayLam))TongNgayLam
    from BV.NHAN_VIEN nv, BV.CHAM_CONG cc
    where nv.MaNV=cc.MaNV
    group by nv.MaNV, nv.TenNV, nv.LuongNV;
/

grant select, update on BV.NHAN_VIEN_KT to KT1;


---------------NV NHAN SU----------------------------------NV NHAN SU----------------------------------NV NHAN SU----------------------------------NV NHAN SU-------------------
/
create user NS1 identified by 1;
grant create session to NS1;
/
--Ma hoa thong tin luong khi nhap vao
grant execute on BV.En_Luong to NS1;
/
--Tao view cho phong ban
create or replace View PHONG_BAN_NS as
    select pb.MaPb, pb.TenPB,bp.MaBP, bp.TenBP, nv.MaNV, nv.TenNV
    from BV.PHONG_BAN pb, BV.NHAN_VIEN nv, BV.BO_PHAN bp
    where pb.MaPB = nv.MaPB and pb.MaBP = bp.MaBP and pb.MaTrPH=nv.MaNV;
/
--Cap quyen tren view
grant select, insert, update on BV.PHONG_BAN_NS to NS1;
/
--Trigger khi insert, update tren View se chuyen sang bang chinh
create or replace trigger Update_PB_View_Trigger
instead of update on BV.PHONG_BAN_NS
begin
    Update BV.PHONG_BAN
    set MaBP = :new.MaBP, MaTrPh=:new.MaNV
    where MaPB=:old.MaPB;
end;
/
create or replace trigger Insert_PB_View_Trigger
instead of insert on BV.PHONG_BAN_NS
begin
    Insert into BV.PHONG_BAN(MaPB, TenPB , MaBP, MaTrPh) Values (:new.MaPB, :new.TenPB, :new.MaBP, :new.MaNV);
end;
/



----Tao view Nhan vien
create or replace View NHAN_VIEN_NS as
    select MaNV, TenNV,LuongNV
    from BV.NHAN_VIEN;
 /   
grant select, update, insert on BV.NHAN_VIEN_NS to NS1;
/
--Tao trigger de insert, update tren View
create or replace trigger Update_NV_View_Trigger
instead of update on BV.NHAN_VIEN_NS
begin
    Update BV.NHAN_VIEN
    set TenNV = :new.TenNV, LuongNV=:new.LuongNV
    where MaNV=:old.MaNV;
end;
/

create or replace trigger Insert_NV_View_Trigger
instead of insert on BV.NHAN_VIEN_NS
begin
    Insert into BV.NHAN_VIEN(MaNV, TenNV , LuongNV)  Values (:new.MaNV, :new.TenNV, :new.LuongNV);
end;
/


--------------NV BAN THUOC --------------------------NV BAN THUOC --------------------------NV BAN THUOC --------------------------NV BAN THUOC ------------


/
create user BT1 identified by 1;
grant create session to BT1;
/
create or replace View Toa_Thuoc_BT as
    select pkt.MaPK, t.TenThuoc, pk.TinhTrang, pkt.SoLuong, t.GiaTien, (pkt.SoLuong*t.GiaTien) as SLxTien
    from BV.THUOC t, BV.PK_THUOC pkt, BV.PHIEU_KHAM pk
    where t.MaThuoc=pkt.MaThuoc and pkt.MaPK=pk.MaPK and pk.TinhTrang=5;
/    
 grant select on  BV.Toa_Thuoc_BT to BT1; 
/

--Ap dung chinh sach VPD, nhan vien chi xem duoc lich cham cong cua chinh nhan vien do
CREATE OR REPLACE FUNCTION BV.POLICY_NHAN_VIEN_XEM_CHAM_CONG(p_schema IN VARCHAR2, p_object IN VARCHAR2)
RETURN VARCHAR2
AS
    CUR_USER VARCHAR2(20):=SYS_CONTEXT ('USERENV', 'SESSION_USER');
BEGIN 
    IF (CUR_USER = 'BV') THEN
        RETURN '';
    ELSIF (CUR_USER = 'KT1') THEN
        RETURN '';
    ELSE    
        RETURN 'MaNV = (SELECT MaNV FROM BV.NHAN_VIEN WHERE USERNAME = '''|| CUR_USER ||''')';
    END IF;
END;
/
--Su dung chinh sach nay len bang cham cong 
BEGIN
DBMS_RLS.ADD_POLICY(
OBJECT_SCHEMA => 'BV',
OBJECT_NAME => 'CHAM_CONG',
POLICY_NAME => 'POL_BV_1',
FUNCTION_SCHEMA => 'BV',
POLICY_FUNCTION => 'POLICY_NHAN_VIEN_XEM_CHAM_CONG',
SEC_RELEVANT_COLS => 'MaNV, NgayLam, TuGio, DenGio',
SEC_RELEVANT_COLS_OPT => DBMS_RLS.ALL_ROWS,
UPDATE_CHECK => true
);
END;
/
