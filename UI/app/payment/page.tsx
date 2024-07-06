'use client'
import React, { useEffect, useState } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import '../ui/globals.css';
const URL = process.env.NEXT_PUBLIC_API;

interface PaymentMethod {
  paymentType: string; // Cambiado a string
  isActive: boolean;
}

const PaymentMethods: React.FC = () => {
  const [paymentMethods, setPaymentMethods] = useState<{ [key: string]: PaymentMethod }>({});

  useEffect(() => {
    fetch(URL + '/api/Payment/payment-methods') // Actualiza con tu endpoint real
      .then(response => response.json())
      .then(data => {
        const methods = Object.keys(data).reduce((acc, key) => {
          acc[key] = { ...data[key] }; // Usar los valores reales de isActive
          return acc;
        }, {} as { [key: string]: PaymentMethod });
        setPaymentMethods(methods);
      });
  }, []);

  const toggleActive = (method: string) => {
    const updatedMethods = {
      ...paymentMethods,
      [method]: {
        ...paymentMethods[method],
        isActive: !paymentMethods[method].isActive,
      },
    };
    setPaymentMethods(updatedMethods);

    fetch(URL + '/api/Payment/update-status', { // Actualiza con tu endpoint real
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        method: method, // Usar el nombre del método como cadena
        state: updatedMethods[method].isActive ? 'activo' : 'desactivado',
      }),
    });
  };

  return (
    <div className="container mt-4">
      <h1 className="text-center">Payment Methods</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>Method</th>
            <th>Type</th>
            <th>Status</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          {Object.keys(paymentMethods).map(method => (
            <tr key={method}>
              <td>{method}</td>
              <td>{paymentMethods[method].paymentType}</td>
              <td>{paymentMethods[method].isActive ? 'Active' : 'Inactive'}</td>
              <td>
                <button
                  className="btn btn-sm btn-primary"
                  onClick={() => toggleActive(method)}
                >
                  {paymentMethods[method].isActive ? 'Deactivate' : 'Activate'}
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default PaymentMethods;
