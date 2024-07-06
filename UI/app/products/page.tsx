'use client';
import React, { useState, useEffect } from 'react';
import '../ui/globals.css';
import 'bootstrap/dist/css/bootstrap.css';
const URL = process.env.NEXT_PUBLIC_API;

const Products: React.FC = () => {
    const [state, setState] = useState({
        productList: [],
        categories: []
    });

    useEffect(() => {
        const fetchData = async () => {
            const response = await fetch(URL + '/api/store');
            if (!response.ok) {
                throw new Error('Failed to fetch data');
            }
            const json = await response.json();
            const { products, categories } = json;

            setState({
                productList: products || [],
                categories: categories || []
            });

            // Save the ID of the last product in local storage
            if (products && products.length > 0) {
                const lastProductId = products[products.length - 1].id;
                localStorage.setItem('lastProductId', lastProductId);
            }
        };

        fetchData();
    }, []);

    const getLastProductId = () => {
        return localStorage.getItem('lastProductId');
    };

    return (
        <div className="products-container">
            <h1 className="products-title">Productos</h1>

            {/* Tabla para mostrar los productos */}
            <table className="table mt-4 my-3">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nombre</th>
                        <th>Precio</th>
                        <th>Descripción</th>
                        <th>Imagen</th>
                    </tr>
                </thead>
                <tbody>
                    {state.productList.map((product: any) => (
                        <tr key={product.id}>
                            <td>{product.id}</td>
                            <td>{product.name}</td>
                            <td>{product.price}</td>
                            <td dangerouslySetInnerHTML={{ __html: product.description }}></td>
                            <td>
                                <img src={product.imageUrl} alt={product.name} style={{ width: '70px', height: '70px' }} />
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            {/* Botón para redirigir a la página de inserción de productos */}
            <button onClick={() => window.location.href = '/products/insertproducts'} className="btn btn-primary float-end">Insertar Producto</button>
        </div>
    );
};

export default Products;
