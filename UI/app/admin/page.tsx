'use client';
import React, { useState } from 'react';
import '../ui/globals.css';
import 'bootstrap/dist/css/bootstrap.css';
import { jwtDecode } from 'jwt-decode';
const URL = process.env.NEXT_PUBLIC_API;
//PROYECTO12

const Admin: React.FC = () => {
  const [formData, setFormData] = useState({
    username: '',
    password: '',
    errorMessage: ''
  });

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value
    });
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const { username, password } = formData;

    const validationResult = validateForm(username, password);

    if (validationResult.isValid) {
      setFormData({
        ...formData,
        errorMessage: ''
      });

      const response = await fetch(URL+'/api/Auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userName: username, userPassword: password })
      });

      const data = await response.json();

      if (response.ok) {
        const decodedToken: any = jwtDecode(data.token);
        const roles = decodedToken?.roles || [];

        if (!roles.includes('Admin')) {
          // Store the token in sessionStorage
          sessionStorage.setItem('authToken', data.token);

          // Redirect the user to the admin page
          window.location.href = '/admin/init';
        } else {
          setFormData({
            ...formData,
            errorMessage: 'Los usuarios sin el rol Admin no pueden iniciar sesión.'
          });
        }
      } else {
        setFormData({
          ...formData,
          errorMessage: data.message || 'Usuario o contraseña incorrectos'
        });
      }
    } else {
      setFormData({
        ...formData,
        errorMessage: validationResult.errorMessage
      });
    }
  };

  const validateForm = (username: string, password: string) => {
    if (!username.trim() || !password.trim()) {
      return {
        isValid: false,
        errorMessage: 'Por favor, complete todos los campos.'
      };
    }
    return {
      isValid: true,
      errorMessage: ''
    };
  };

  return (
    <div>
      <h1>Iniciar Sesión</h1>
      <form onSubmit={handleSubmit}>
        <div>
          <label className="col-form-label" htmlFor="username">
            Nombre de Usuario:
          </label>
          <input
            type="text"
            id="username"
            name="username"
            value={formData.username}
            onChange={handleInputChange}
          />
        </div>
        <div>
          <label className="col-form-label" htmlFor="password">
            Contraseña:
          </label>
          <input
            type="password"
            id="password"
            name="password"
            value={formData.password}
            onChange={handleInputChange}
          />
        </div>
        <div className="center-button">
          <button className="btn btn-primary my-4" type="submit">
            Iniciar Sesión
          </button>
        </div>
      </form>
      {formData.errorMessage && (
        <p style={{ color: 'red' }}>{formData.errorMessage}</p>
      )}
    </div>
  );
};

export default Admin;
