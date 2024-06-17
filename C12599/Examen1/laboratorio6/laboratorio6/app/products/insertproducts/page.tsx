'use client'
import React, { useState } from 'react';
import '../../ui/globals.css';

const categories = [
    { id: 1, name: "Electrónica" },
    { id: 2, name: "Moda" },
    { id: 3, name: "Hogar y jardín" },
    { id: 4, name: "Deportes y actividades al aire libre" },
    { id: 5, name: "Belleza y cuidado personal" },
    { id: 6, name: "Alimentación y bebidas" },
    { id: 7, name: "Libros y entretenimiento" },
    { id: 8, name: "Tecnología" },
    { id: 9, name: "Deportes" }
];

const InsertProduct = () => {
    const [productName, setProductName] = useState('');
    const [productPrice, setProductPrice] = useState('');
    const [productDescription, setProductDescription] = useState('');
    const [productImage, setProductImage] = useState('');
    const [productCategory, setProductCategory] = useState(categories[0].id);
    const [errors, setErrors] = useState({});

    const validateForm = () => {
        let formErrors = {};

        if (!productName.trim()) {
            formErrors.productName = 'El nombre del producto es obligatorio';
        }

        if (!productPrice || isNaN(productPrice) || parseFloat(productPrice) <= 0) {
            formErrors.productPrice = 'El precio del producto debe ser un número positivo';
        }

        if (!productDescription.trim()) {
            formErrors.productDescription = 'La descripción del producto es obligatoria';
        }

        if (!productImage.trim()) {
            formErrors.productImage = 'La URL de la imagen del producto es obligatoria';
        }

        setErrors(formErrors);
        return Object.keys(formErrors).length === 0;
    };

    const handleSubmit = async (event) => {
        event.preventDefault();

        if (!validateForm()) {
            return;
        }

        const token = sessionStorage.getItem('authToken');

        // Get the last product ID from local storage and set the new product ID
        const lastProductId = localStorage.getItem('lastProductId');
        const newProductId = lastProductId ? parseInt(lastProductId) + 1 : 1;

        const payload = {
            id: newProductId,
            name: productName,
            imageUrl: productImage,
            price: parseFloat(productPrice),
            description: productDescription,
            categoryId: productCategory
        };

        const response = await fetch('https://localhost:7043/api/InsertProducts', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            // Save the new product ID to local storage
            localStorage.setItem('lastProductId', newProductId.toString());
            // Clear the form fields
            setProductName('');
            setProductPrice('');
            setProductDescription('');
            setProductImage('');
            setProductCategory(categories[0].id);
        }
    };

    const handleGoBack = () => {
        window.location.href = '/products';
    };

    return (
        <div className="insert-product-container">
            <h1 className="insert-product-title">Insertar Producto</h1>

            <form onSubmit={handleSubmit} className="insert-product-form">
                <div className="form-group">
                    <label htmlFor="productName" className="insert-product-label">Nombre del Producto:</label>
                    <input 
                        type="text" 
                        id="productName" 
                        className="form-control" 
                        value={productName} 
                        onChange={(e) => setProductName(e.target.value)} 
                        required 
                    />
                    {errors.productName && <span className="error-message">{errors.productName}</span>}
                </div>

                <div className="form-group">
                    <label htmlFor="productPrice" className="insert-product-label">Precio del Producto:</label>
                    <input 
                        type="number" 
                        step="0.01" 
                        id="productPrice" 
                        className="form-control" 
                        value={productPrice} 
                        onChange={(e) => setProductPrice(e.target.value)} 
                        required 
                    />
                    {errors.productPrice && <span className="error-message">{errors.productPrice}</span>}
                </div>

                <div className="form-group">
                    <label htmlFor="productDescription" className="insert-product-label">Descripción del Producto:</label>
                    <textarea 
                        id="productDescription" 
                        className="form-control" 
                        value={productDescription} 
                        onChange={(e) => setProductDescription(e.target.value)} 
                        required 
                    />
                    {errors.productDescription && <span className="error-message">{errors.productDescription}</span>}
                </div>

                <div className="form-group">
                    <label htmlFor="productImage" className="insert-product-label">Imagen del Producto:</label>
                    <input 
                        type="text" 
                        id="productImage" 
                        className="form-control" 
                        value={productImage} 
                        onChange={(e) => setProductImage(e.target.value)} 
                        required 
                    />
                    {errors.productImage && <span className="error-message">{errors.productImage}</span>}
                </div>

                <div className="form-group">
                    <label htmlFor="productCategory" className="insert-product-label">Categoría del Producto:</label>
                    <select 
                        id="productCategory" 
                        className="form-control" 
                        value={productCategory} 
                        onChange={(e) => setProductCategory(parseInt(e.target.value))} 
                        required
                    >
                        {categories.map(category => (
                            <option key={category.id} value={category.id}>{category.name}</option>
                        ))}
                    </select>
                </div>

                <button type="submit" className="btn btn-primary">Insertar Producto</button>
            </form>

            <button onClick={handleGoBack} className="btn btn-secondary mt-3">Volver a la Página Principal</button>
        </div>
    );
};

export default InsertProduct;
