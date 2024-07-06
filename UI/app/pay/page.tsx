'use client'
import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import 'bootstrap/dist/css/bootstrap.css';
import '../ui/globals.css';
//PROYECTO12

const URL = process.env.NEXT_PUBLIC_API;

interface Cart {
    productos: any[];
    direccionEntrega: string;
    metodoPago: string;
    isCartEmpty: boolean;
    numeroCompra: string;
    sinpeAvailable: boolean;
    cashAvailable: boolean;
}

const PayPage: React.FC = () => {
    const [cart, setCart] = useState<Cart>({
        productos: [],
        direccionEntrega: '',
        metodoPago: '',
        isCartEmpty: true,
        numeroCompra: '',
        sinpeAvailable: false,
        cashAvailable: false
    });

    const [direccionValida, setDireccionValida] = useState(false);
    const [formDireccion, setFormDireccion] = useState({
        nombreDestinatario: '',
        direccionCalle: '',
        direccionNumero: '',
        direccionCiudad: '',
        direccionPais: ''
    });

    const [errores, setErrores] = useState({
        nombreDestinatario: '',
        direccionCalle: '',
        direccionNumero: '',
        direccionCiudad: '',
        direccionPais: ''
    });

    useEffect(() => {
        const loadCartData = async () => {
            let cartDataString = localStorage.getItem('cartData');
            if (cartDataString !== null) {
                let cartData = JSON.parse(cartDataString);
                const tieneProductosValidos = cartData && Array.isArray(cartData.productos) && cartData.productos.length > 0;
                if (tieneProductosValidos) {
                    setCart({ ...cartData, isCartEmpty: false });
                } else {
                    localStorage.removeItem('cartData');
                    setCart(prevCart => ({ ...prevCart, isCartEmpty: true }));
                }
            } else {
                setCart(prevCart => ({ ...prevCart, isCartEmpty: true }));
            }
        };

        loadCartData();
    }, []);

    useEffect(() => {
        const fetchPaymentMethods = async () => {
            const response = await fetch(URL + '/api/Payment/payment-methods');
            if (response.ok) {
                const paymentMethodsData = await response.json();
                const { cash, sinpe } = paymentMethodsData;

                setCart(prevCart => ({
                    ...prevCart,
                    cashAvailable: cash.isActive,
                    sinpeAvailable: sinpe.isActive
                }));
            } else {
                throw new Error('Error al obtener métodos de pago');
            }
        };

        fetchPaymentMethods();
    }, []);

    useEffect(() => {
        const { nombreDestinatario, direccionCalle, direccionNumero, direccionCiudad, direccionPais } = formDireccion;
        const allFieldsFilled = nombreDestinatario && direccionCalle && direccionNumero && direccionCiudad && direccionPais;
        setDireccionValida(allFieldsFilled);
    }, [formDireccion]);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { id, value } = e.target;
        setFormDireccion(prevState => ({ ...prevState, [id]: value.trim() }));
        setErrores(prevState => ({ ...prevState, [id]: '' })); // Clear error message when user starts typing
    };

    const handleAgregarDireccion = () => {
        const newErrors = { ...errores };

        if (formDireccion.nombreDestinatario.length < 5) {
            newErrors.nombreDestinatario = 'El nombre del destinatario debe tener al menos 5 caracteres.';
        } else if (!/^[a-zA-Z\sñÑáéíóúÁÉÍÓÚ]+$/.test(formDireccion.nombreDestinatario)) {
            newErrors.nombreDestinatario = 'El nombre solo debe contener letras y espacios.';
        } else {
            newErrors.nombreDestinatario = '';
        }

        if (formDireccion.direccionCalle.length < 5) {
            newErrors.direccionCalle = 'La calle debe tener al menos 5 caracteres.';
        } else if (!/^[a-zA-Z0-9\sñÑáéíóúÁÉÍÓÚ.,'-]+$/.test(formDireccion.direccionCalle)) {
            newErrors.direccionCalle = 'La dirección contiene caracteres inválidos.';
        } else {
            newErrors.direccionCalle = '';
        }

        if (formDireccion.direccionNumero.length < 1) {
            newErrors.direccionNumero = 'El número debe tener al menos 1 carácter.';
        } else if (!/^\d+[a-zA-Z]?$/i.test(formDireccion.direccionNumero)) {
            newErrors.direccionNumero = 'El número de la casa es inválido.';
        } else {
            newErrors.direccionNumero = '';
        }

        if (formDireccion.direccionCiudad.length < 3) {
            newErrors.direccionCiudad = 'La ciudad debe tener al menos 3 caracteres.';
        } else if (!/^[a-zA-Z\sñÑáéíóúÁÉÍÓÚ]+$/.test(formDireccion.direccionCiudad)) {
            newErrors.direccionCiudad = 'El nombre de la ciudad solo debe contener letras y espacios.';
        } else {
            newErrors.direccionCiudad = '';
        }

        if (formDireccion.direccionPais.length < 3) {
            newErrors.direccionPais = 'El país debe tener al menos 3 caracteres.';
        } else if (!/^[a-zA-Z\sñÑáéíóúÁÉÍÓÚ]+$/.test(formDireccion.direccionPais)) {
            newErrors.direccionPais = 'El nombre del país solo debe contener letras y espacios.';
        } else {
            newErrors.direccionPais = '';
        }
        setErrores(newErrors);

        const allValid = Object.values(newErrors).every(error => error === '');
        if (allValid) {
            const direccionCompleta = `${formDireccion.nombreDestinatario}, ${formDireccion.direccionCalle}, ${formDireccion.direccionNumero}, ${formDireccion.direccionCiudad}, ${formDireccion.direccionPais}`;
            setCart(prevCart => ({ ...prevCart, direccionEntrega: direccionCompleta }));
            updateLocalStorage({ ...cart, direccionEntrega: direccionCompleta });
            setDireccionValida(true); // Set the address as valid
        }
    };

    const handleMetodoPagoChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const value = e.target.value.trim();
        if (!value) {
            throw new Error('Seleccione un método de pago válido.');
        }
        setCart(prevCart => ({ ...prevCart, metodoPago: value }));
        updateLocalStorage({ ...cart, metodoPago: value });
    };

    const updateLocalStorage = (updatedCart: Cart) => {
        localStorage.setItem('cartData', JSON.stringify(updatedCart));
    };

    const handleSubmit = async () => {
        const { direccionEntrega, metodoPago, productos } = cart;

        if (!direccionEntrega || !metodoPago) {
            throw new Error('Por favor, complete todos los campos requeridos.');
        }

        const isValidPaymentMethod = metodoPago === 'sinpe' && cart.sinpeAvailable || metodoPago === 'efectivo' && cart.cashAvailable;
        if (!isValidPaymentMethod) {
            throw new Error('Método de pago no válido o no disponible.');
        }

        const productIds = productos.map((producto: any) => String(producto.id));

        let paymentMethodValue = metodoPago === 'sinpe' ? 1 : 0;

        const dataToSend = {
            productIds: productIds,
            address: direccionEntrega,
            paymentMethod: paymentMethodValue
        };

        const response = await fetch(URL + '/api/Cart', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(dataToSend)
        });

        if (response.ok) {
            const responseData = await response.json();
            const { purchaseNumberResponse } = responseData;

            setCart({
                productos: [],
                direccionEntrega: '',
                metodoPago: '',
                isCartEmpty: true,
                numeroCompra: purchaseNumberResponse.toString(),
                sinpeAvailable: false,
                cashAvailable: false
            });

            updateLocalStorage({
                productos: [],
                direccionEntrega: '',
                metodoPago: '',
                isCartEmpty: true,
                numeroCompra: purchaseNumberResponse.toString(),
                sinpeAvailable: false,
                cashAvailable: false
            });
        } else {
            const errorResponseData = await response.json();
            throw new Error('Error al enviar datos de pago: ' + JSON.stringify(errorResponseData));
        }
    };

    return (
        <div>
            <h1>Pay Page</h1>
            {cart.isCartEmpty ? (
                <p>Su carrito está vacío. Por favor, agregue productos antes de proceder al pago.</p>
            ) : (
                <>
                    <div className="form-group">
                        <label htmlFor="nombreDestinatario">Nombre del Destinatario:</label>
                        <input
                            type="text"
                            id="nombreDestinatario"
                            className="form-control"
                            value={formDireccion.nombreDestinatario}
                            onChange={handleInputChange}
                        />
                        {errores.nombreDestinatario && <small className="text-danger">{errores.nombreDestinatario}</small>}
                    </div>
                    <div className="form-group">
                        <label htmlFor="direccionCalle">Calle:</label>
                        <input
                            type="text"
                            id="direccionCalle"
                            className="form-control"
                            value={formDireccion.direccionCalle}
                            onChange={handleInputChange}
                        />
                        {errores.direccionCalle && <small className="text-danger">{errores.direccionCalle}</small>}
                    </div>
                    <div className="form-group">
                        <label htmlFor="direccionNumero">Número:</label>
                        <input
                            type="text"
                            id="direccionNumero"
                            className="form-control"
                            value={formDireccion.direccionNumero}
                            onChange={handleInputChange}
                        />
                        {errores.direccionNumero && <small className="text-danger">{errores.direccionNumero}</small>}
                    </div>
                    <div className="form-group">
                        <label htmlFor="direccionCiudad">Ciudad:</label>
                        <input
                            type="text"
                            id="direccionCiudad"
                            className="form-control"
                            value={formDireccion.direccionCiudad}
                            onChange={handleInputChange}
                        />
                        {errores.direccionCiudad && <small className="text-danger">{errores.direccionCiudad}</small>}
                    </div>
                    <div className="form-group">
                        <label htmlFor="direccionPais">País:</label>
                        <input
                            type="text"
                            id="direccionPais"
                            className="form-control"
                            value={formDireccion.direccionPais}
                            onChange={handleInputChange}
                        />
                        {errores.direccionPais && <small className="text-danger">{errores.direccionPais}</small>}
                    </div>

                    <div className='my-4'>
                        <button className="btn btn-primary" onClick={handleAgregarDireccion}>Agregar Dirección</button>
                    </div>

                    {direccionValida && (
                        <>
                            <div className="form-group">
                                <label htmlFor="metodoPago">Método de Pago:</label>
                                <select
                                    id="metodoPago"
                                    className="form-control"
                                    value={cart.metodoPago}
                                    onChange={handleMetodoPagoChange}
                                >
                                    <option value="">Seleccione un método de pago</option>
                                    {cart.sinpeAvailable && <option value="sinpe">Sinpe</option>}
                                    {cart.cashAvailable && <option value="efectivo">Efectivo</option>}
                                </select>
                            </div>
                            {cart.metodoPago && (
                                <>
                                    {cart.metodoPago === 'sinpe' && (
                                        <Link href="/confirmacionSinpe">
                                            <button className="btn btn-primary" onClick={handleSubmit}>Ir a Confirmación Sinpe</button>
                                        </Link>
                                    )}
                                    {cart.metodoPago === 'efectivo' && (
                                        <Link href="/confirmacionEfectivo">
                                            <button className="btn btn-primary" onClick={handleSubmit}>Ir a Confirmación Efectivo</button>
                                        </Link>
                                    )}
                                </>
                            )}
                        </>
                    )}
                </>
            )}
        </div>
    );
}

export default PayPage;
