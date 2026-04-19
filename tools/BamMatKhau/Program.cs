// Tool nho: in ra hash BCrypt cua mat khau truyen vao de reset mat khau qua SQL.
// Cach dung:
//   cd tools/BamMatKhau
//   dotnet run -- "123456"
// Sau do copy hash in ra, chay SQL:
//   UPDATE TaiKhoan SET MatKhau = '<hash>' WHERE TenDangNhap = '<ten>';

if (args.Length == 0)
{
    Console.Error.WriteLine("Su dung:");
    Console.Error.WriteLine("  Tao hash:  dotnet run -- \"<mat khau moi>\"");
    Console.Error.WriteLine("  Verify:    dotnet run -- verify \"<mat khau>\" \"<hash>\"");
    return 1;
}

if (args[0] == "verify")
{
    if (args.Length < 3)
    {
        Console.Error.WriteLine("Verify can 2 tham so: mat khau va hash.");
        return 1;
    }
    var ok = BCrypt.Net.BCrypt.Verify(args[1], args[2]);
    Console.WriteLine($"Mat khau \"{args[1]}\" vs hash -> {(ok ? "KHOP" : "KHONG khop")}");
    return 0;
}

var matKhau = args[0];
var hash = BCrypt.Net.BCrypt.HashPassword(matKhau, workFactor: 11);

Console.WriteLine();
Console.WriteLine("Mat khau : " + matKhau);
Console.WriteLine("Hash     : " + hash);
Console.WriteLine();
Console.WriteLine("SQL mau de reset (thay <TEN_DANG_NHAP> bang ten cua ban):");
Console.WriteLine();
Console.WriteLine($"  UPDATE TaiKhoan SET MatKhau = '{hash}' WHERE TenDangNhap = '<TEN_DANG_NHAP>';");
Console.WriteLine();
return 0;
