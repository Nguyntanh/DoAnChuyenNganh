// Mảng chứa các URL của ảnh
const images = [
    'landing_assets/images/image1.png',  // Ảnh đầu tiên
    'landing_assets/images/image2.png',  // Ảnh thứ hai
    'landing_assets/images/image3.png'
    // 'assets/images/image4.png'   // Ảnh thứ ba (thêm ảnh mới)
];

let currentImageIndex = 0; // Đặt chỉ số ảnh hiện tại là 0 (ảnh đầu tiên)

// Khi trang web tải, thiết lập ảnh nền đầu tiên
document.querySelector('.swap').style.backgroundImage = `url("${images[currentImageIndex]}")`;

document.querySelector('.turn_left').addEventListener('click', function() {
    // Giảm chỉ số ảnh (quay lại ảnh trước đó)
    currentImageIndex = (currentImageIndex - 1 + images.length) % images.length;
    // Thay đổi ảnh nền
    document.querySelector('.swap').style.backgroundImage = `url("${images[currentImageIndex]}")`;
});

document.querySelector('.turn_right').addEventListener('click', function() {
    // Tăng chỉ số ảnh (chuyển đến ảnh tiếp theo)
    currentImageIndex = (currentImageIndex + 1) % images.length;
    // Thay đổi ảnh nền
    document.querySelector('.swap').style.backgroundImage = `url("${images[currentImageIndex]}")`;
});
document.addEventListener("DOMContentLoaded", function () {
    const wrapper = document.querySelector(".product_wrapper");
    const btnLeft = document.querySelector(".turn_left_product");
    const btnRight = document.querySelector(".turn_right_product");
    
    const scrollAmount = wrapper.clientWidth; 

    btnLeft.addEventListener("click", () => {
        wrapper.scrollLeft -= scrollAmount; // Cuộn sang trái
    });

    btnRight.addEventListener("click", () => {
        wrapper.scrollLeft += scrollAmount; // Cuộn sang phải
    });
});

    document.addEventListener("DOMContentLoaded", function () {
        let dropdowns = document.querySelectorAll(".navbar ul li");

        dropdowns.forEach((dropdown) => {
            dropdown.addEventListener("click", function (event) {
                let submenu = this.querySelector(".dropdown");
                if (submenu) {
                    submenu.style.display = submenu.style.display === "block" ? "none" : "block";
                    event.stopPropagation();
                }
            });
        });

        document.addEventListener("click", function () {
            document.querySelectorAll(".dropdown").forEach((dropdown) => {
                dropdown.style.display = "none";
            });
        });
    });
