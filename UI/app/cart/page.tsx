'use client';
import React, { useState, useEffect } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import '../ui/globals.css';
import Link from 'next/link';
//PROYECTO

type CartItem = {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
};

type Cart = {
  productos: CartItem[];
  subtotal: number;
  total: number;
};

const EjemploPage: React.FC = () => {
  const [carrito, setCarrito] = useState<Cart>({
    productos: [],
    subtotal: 0,
    total: 0
  });

  useEffect(() => {
    const cartData = JSON.parse(localStorage.getItem('cartData') || '{}');
    if (cartData.productos) {
      setCarrito(cartData);
    }
  }, []);

  useEffect(() => {
    let hayProductosEnCarrito = carrito.productos && carrito.productos.length > 0;
    if (hayProductosEnCarrito) {
      let subtotalCalculado = carrito.productos.reduce((total, item) => total + item.price, 0);
      let totalCalculado = (subtotalCalculado * 0.13) + subtotalCalculado;
      setCarrito(prevCarrito => ({
        ...prevCarrito,
        subtotal: subtotalCalculado,
        total: totalCalculado
      }));
    }
  }, [carrito.productos]);

  const handleClearCart = () => {
    const emptyCart = {
      productos: [],
      subtotal: 0,
      total: 0
    };
    localStorage.setItem('cartData', JSON.stringify(emptyCart));
    setCarrito(emptyCart);
  };

  const handleContinueToPayment = () => {
    const subtotalCalculado = carrito.productos.reduce((total, item) => total + item.price, 0);
    const totalCalculado = (subtotalCalculado * 0.13) + subtotalCalculado;

    if (!subtotalCalculado || !totalCalculado) {
      throw new Error('No se pueden calcular los totales del carrito.');
    }

    const updatedCartData = {
      ...carrito,
      subtotal: subtotalCalculado,
      total: totalCalculado
    };

    localStorage.setItem('cartData', JSON.stringify(updatedCartData));
  };

  return (
    <div>
      <h1>Carrito de Compras</h1>
      <div className="row my-3">
        {carrito.productos && carrito.productos.length > 0 && carrito.productos.map(item => (
          <div key={item.id} className="col-sm-3 mb-4">
            <div className="card">
              <img src={item.imageUrl} className="card-img-top" alt={item.name} />
              <div className="card-body">
                <div className="text-center">
                  <h5 className="card-title my-3">{item.name}</h5>
                  <p className="card-text my-3">{item.description}</p>
                  <p className="card-text my-3">Precio: ${item.price}</p>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>
      <div className="text-right">
        <h2>Subtotal: ${carrito.subtotal ? carrito.subtotal.toFixed(2) : '0.00'}</h2>
        <h2 className='my-3'>Total: ${carrito.total ? carrito.total.toFixed(2) : '0.00'}</h2>
      </div>
      <div className="text-right">
        {carrito.productos && carrito.productos.length > 0 && (
          <>
            <button className="btn btn-danger mr-3" onClick={handleClearCart}>Vaciar Carrito</button>
            <Link href="/pay">
              <button className="btn btn-primary" onClick={handleContinueToPayment}>Continuar Compra</button>
            </Link>
          </>
        )}
      </div>
    </div>
  );
}

export default EjemploPage;
