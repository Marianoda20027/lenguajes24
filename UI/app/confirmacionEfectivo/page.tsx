'use client'
import React, { useState, useEffect } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import '../ui/globals.css';

const ConfirmacionEfectivoPage = () => {
    const [formState, setFormState] = useState({
        comprobante: '',
        confirmado: false,
        numeroCompra: 'Cargando...'
    });

    useEffect(() => {
        const cartDataString = localStorage.getItem('cartData');
        if (!cartDataString) {
            throw new Error('No hay datos de carrito disponibles.');
        }

        const cartData = JSON.parse(cartDataString);

        setFormState(prevState => ({
            ...prevState,
            numeroCompra: cartData.numeroCompra
        }));
    }, []);

    const handleConfirmar = () => {


        localStorage.removeItem('cartData');
        setFormState(prevState => ({
            ...prevState,
            confirmado: true
        }));
    };

    const handleVolver = () => {
        window.location.href = '/';
    };

    return (
        <div className="container mt-5">
            <h1 className="mb-4">Confirmación Efectivo</h1>
            <p className="mb-4">Número de Compra: {formState.numeroCompra}</p>
            <div className="mb-4">
                <button className="btn btn-primary me-3" onClick={handleConfirmar}>
                    Confirmar
                </button>
            </div>

            {formState.confirmado && (
                <div className="mb-4">
                    <p>Esperando confirmación del administrador...</p>
                </div>
            )}

            {formState.confirmado && (
                <div>
                    <button className="btn btn-secondary" onClick={handleVolver}>
                        Volver a página principal
                    </button>
                </div>
            )}
        </div>
    );
};

export default ConfirmacionEfectivoPage;
