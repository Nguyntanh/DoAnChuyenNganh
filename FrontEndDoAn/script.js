// Lấy giỏ hàng từ localStorage
function getCart() {
    return JSON.parse(localStorage.getItem("cart")) || [];
}

// Lưu giỏ hàng vào localStorage
function saveCart(cart) {
    localStorage.setItem("cart", JSON.stringify(cart));
}

// Cập nhật giao diện giỏ hàng
function updateCart() {
    const cartTable = document.getElementById("cart-items");
    if (!cartTable) return; // Nếu không có bảng giỏ hàng, thoát

    cartTable.innerHTML = "";
    let cartData = getCart();
    let totalItems = 0;
    let totalPrice = 0;

    cartData.forEach((product, index) => {
        totalItems += product.quantity;
        totalPrice += product.quantity * product.price;

        const row = document.createElement("tr");
        row.innerHTML = `
            <td><img src="${product.img}" width="50"></td>
            <td>${product.name}</td>
            <td>${product.quantity}</td>
            <td>${product.price.toLocaleString()} VNĐ</td>
            <td>${(product.quantity * product.price).toLocaleString()} VNĐ</td>
            <td><button onclick="removeItem(${index})">❌</button></td>
        `;
        cartTable.appendChild(row);
    });

    document.getElementById("total-items").textContent = totalItems;
    document.getElementById("total-price").textContent = totalPrice.toLocaleString() + " VNĐ";
}

// Thêm vào giỏ hàng và chuyển trang
function addToCartAndRedirect(event) {
    event.preventDefault(); // Ngăn chặn hành vi mặc định của thẻ <a>

    // Lấy đúng thông tin sản phẩm đang hiển thị
    let name = document.querySelector('.product_info h1').textContent.trim();
    let price = parseInt(document.getElementById('product-price').dataset.price);
    let img = document.querySelector('.product-img').src;
    let quantity = parseInt(document.getElementById('quantity').value) || 1;

    let cartData = getCart();
    let existingProduct = cartData.find(product => product.name === name);

    if (existingProduct) {
        existingProduct.quantity += quantity; // Nếu có rồi thì tăng số lượng
    } else {
        cartData.push({ img, name, quantity, price });
    }

    saveCart(cartData); // Lưu vào localStorage
    window.location.href = "index.html"; // Chuyển hướng sang trang giỏ hàng
}


// Xóa sản phẩm khỏi giỏ hàng
function removeItem(index) {
    let cartData = getCart();
    cartData.splice(index, 1);
    saveCart(cartData);
    updateCart();
}

// Áp dụng giảm giá
function applyDiscount() {
    let discount = parseInt(document.getElementById("discount-code").value) || 0;
    let cartData = getCart();
    let totalPrice = cartData.reduce((sum, product) => sum + product.quantity * product.price, 0);
    let discountAmount = (totalPrice * discount) / 100;
    let finalPrice = totalPrice - discountAmount;
    document.getElementById("total-price").textContent = finalPrice.toLocaleString() + " VNĐ";
}

// Mở overlay thanh toán
function openOverlay() {
    const paymentItems = document.getElementById("payment-items");
    paymentItems.innerHTML = "";

    let cartData = getCart();
    cartData.forEach((product, index) => {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td><input type="checkbox" value="${index}"></td>
            <td>${product.name}</td>
            <td>${product.quantity}</td>
            <td>${product.price.toLocaleString()} VNĐ</td>
        `;
        paymentItems.appendChild(row);
    });

    document.getElementById("payment-overlay").style.display = "flex";
}

// Đóng overlay
function closeOverlay() {
    document.getElementById("payment-overlay").style.display = "none";
}

// Xác nhận thanh toán
function confirmPayment() {
    const selectedProducts = [];
    document.querySelectorAll('input[type="checkbox"]:checked').forEach((checkbox) => {
        selectedProducts.push(getCart()[checkbox.value]);
    });

    if (selectedProducts.length === 0) {
        alert("Vui lòng chọn ít nhất một sản phẩm để thanh toán.");
        return;
    }

    console.log("Thanh toán các sản phẩm:", selectedProducts);
    alert("Thanh toán thành công!");
    closeOverlay();
}

// Khi tải trang, cập nhật giỏ hàng
document.addEventListener("DOMContentLoaded", updateCart);

function confirmPayment() {
    let cartData = getCart();
    const selectedIndexes = [];

    document.querySelectorAll('input[type="checkbox"]:checked').forEach((checkbox) => {
        selectedIndexes.push(parseInt(checkbox.value, 10));
    });

    if (selectedIndexes.length === 0) {
        alert("Vui lòng chọn ít nhất một sản phẩm để thanh toán.");
        return;
    }

    // Xóa các sản phẩm đã chọn khỏi giỏ hàng
    cartData = cartData.filter((_, index) => !selectedIndexes.includes(index));

    // Lưu lại giỏ hàng mới sau khi xóa
    saveCart(cartData);
    updateCart(); // Cập nhật giao diện giỏ hàng
    closeOverlay(); // Đóng overlay
    alert("Thanh toán thành công!");
}
