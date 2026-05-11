# Chạy ứng dụng với log chi tiết (thường dùng Serilog trong code của bạn)
run:
	dotnet run --project API --configuration Debug

# Build kèm theo thông báo mức độ chi tiết (Minimal/Normal/Detailed)
build:
	dotnet build -v m

clean:
	dotnet clean

# Thêm cờ --verbose để xem lệnh SQL "bay" vào database
update-db:
	dotnet ef database update --project Infrastructure --startup-project API --verbose

# Thêm cờ --verbose để xem quá trình quét Entity
add-Migration:
	dotnet ef migrations add $(name) --project Infrastructure --startup-project API --verbose

# Xóa database kèm xác nhận chi tiết
delete-db:
	dotnet ef database drop -f --project Infrastructure --startup-project API --verbose

# Lệnh reset bao gồm cả dọn dẹp migration cũ (nếu bạn muốn làm lại từ đầu)
reset-db:
	dotnet ef database drop -f --project Infrastructure --startup-project API
	dotnet ef database update --project Infrastructure --startup-project API
remove-db:
	dotnet ef migrations remove --project Infrastructure --startup-project API
check-dirty:
	git diff --quiet || (echo "⚠️ You have unsaved/unstaged changes!" && exit 1)

safe-run:
	dotnet clean
	dotnet build || exit 1
	dotnet run --project API