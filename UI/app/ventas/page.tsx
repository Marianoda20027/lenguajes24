"use client";
import React, { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { PieChart, Pie, Legend, Tooltip, Cell } from 'recharts';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import 'bootstrap/dist/css/bootstrap.css';
import '../ui/globals.css';
import { jwtDecode } from 'jwt-decode';
import VerifyComponent from '../components/VerifyToken';

const Ventas = () => {
    const [state, setState] = useState({
        transactionsDays: [],
        selectedDate: new Date(),
        pieChartData: [],
        errorMessage: '',
    });

    const [isVerified, setIsVerified] = useState(false);
    const router = useRouter();

    const { transactionsDays, selectedDate, pieChartData, errorMessage } = state;

    useEffect(() => {
        const verifyToken = () => {
            const token = typeof window !== 'undefined' ? sessionStorage.getItem('authToken') : null;
            if (!token) {
                router.push('/admin');
                return;
            }

            const decodedToken = jwtDecode(token);
            const exp = decodedToken.exp * 1000;
            const userRole = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

            if (Date.now() >= exp || userRole !== "Admin") {
                sessionStorage.removeItem('authToken');
                router.push('/admin');
            } else {
                setIsVerified(true);
            }
        };

        verifyToken();
    }, [router]);

    useEffect(() => {
        if (isVerified) {
            fetchData();
        }
    }, [isVerified, selectedDate]);

    const fetchData = async () => {
        if (!selectedDate) return;

        const token = sessionStorage.getItem('authToken');
        const formattedDate = selectedDate.toISOString().split('T')[0];
        const url = `https://localhost:7043/api/Sales/transactions?date=${formattedDate}`;

        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            const errorMsg = response.status === 403 ? 'No tiene permisos de administrador' : 'Error al obtener los datos.';
            setState(prevState => ({
                ...prevState,
                errorMessage: errorMsg,
            }));
            return;
        }

        const json = await response.json();
        const pieData = generatePieChartData(json.transactionsWeeks || []);

        setState(prevState => ({
            ...prevState,
            transactionsDays: json.transactionsDays || [],
            pieChartData: pieData,
            errorMessage: '',
        }));
    };

    const generatePieChartData = (transactions) => {
        if (!transactions || !Array.isArray(transactions)) {
            throw new Error('Los datos de transacciones no son válidos.');
        }

        const countByDayOfWeek = {};
        transactions.forEach(transaction => {
            const dayOfWeek = new Date(transaction.transactionDate).getDay();
            countByDayOfWeek[dayOfWeek] = (countByDayOfWeek[dayOfWeek] || 0) + 1;
        });

        const totalTransactions = transactions.length;

        return Object.keys(countByDayOfWeek).map(dayOfWeek => {
            const dayName = getDayName(parseInt(dayOfWeek, 10));
            const percentage = (countByDayOfWeek[dayOfWeek] / totalTransactions) * 100;
            return {
                name: `${dayName} (${percentage.toFixed(2)}%)`,
                value: countByDayOfWeek[dayOfWeek],
                color: getRandomColor(),
            };
        });
    };

    const getDayName = (dayOfWeek) => {
        const days = ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'];
        if (typeof dayOfWeek !== 'number' || dayOfWeek < 0 || dayOfWeek > 6) {
            throw new Error('El argumento dayOfWeek debe ser un número entre 0 y 6.');
        }
        return days[dayOfWeek];
    };

    const getRandomColor = () => {
        return `hsl(${Math.random() * 360}, 70%, 50%)`;
    };

    const handleDateChange = (date) => {
        if (!date) {
            throw new Error('La fecha seleccionada no es válida.');
        }
        const adjustedDate = new Date(date);
        adjustedDate.setDate(adjustedDate.getDate());
        setState(prevState => ({
            ...prevState,
            selectedDate: adjustedDate,
        }));
    };

    const formattedDisplayDate = new Date(selectedDate);
    formattedDisplayDate.setDate(formattedDisplayDate.getDate());

    if (!isVerified) {
        return null;
    }

    return (
        <VerifyComponent>
            <div>
                <h1 className="text-center">Gráfico de Ventas</h1>
                {errorMessage ? (
                    <p className="text-center text-danger">{errorMessage}</p>
                ) : (
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                        <div style={{ flex: '1', marginRight: '20px' }}>
                            <h2>Gráfico semanal</h2>
                            {pieChartData.length > 0 ? (
                                <PieChart width={400} height={550}>
                                    <Pie
                                        dataKey="value"
                                        data={pieChartData}
                                        cx={200}
                                        cy={150}
                                        outerRadius={100}
                                        innerRadius={60}
                                        label
                                    >
                                        {pieChartData.map((entry, index) => (
                                            <Cell key={`cell-${index}`} fill={entry.color} />
                                        ))}
                                    </Pie>
                                    <Tooltip formatter={(value, name) => [value, name]} />
                                    <Legend />
                                </PieChart>
                            ) : (
                                <p>No hay datos disponibles para mostrar el gráfico.</p>
                            )}
                        </div>

                        <div style={{ flex: '1' }}>
                            <h2>Tabla diaria</h2>
                            <div style={{ marginBottom: '20px' }}>
                                <DatePicker
                                    selected={formattedDisplayDate}
                                    onChange={handleDateChange}
                                    dateFormat="yyyy-MM-dd"
                                    placeholderText="Selecciona una fecha"
                                />
                            </div>

                            <table className="table">
                                <thead>
                                    <tr>
                                        <th>Número de Compra</th>
                                        <th>Monto Total</th>
                                        <th>Fecha de Transacción</th>
                                        <th>Productos</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {transactionsDays.map((transaction, index) => (
                                        <tr key={index}>
                                            <td>{transaction.purchaseNumber}</td>
                                            <td>{transaction.totalAmount}</td>
                                            <td>{new Date(transaction.transactionDate).toLocaleDateString()}</td>
                                            <td>
                                                <ul>
                                                    {transaction.products.map((product, index) => (
                                                        <li key={index}>{product}</li>
                                                    ))}
                                                </ul>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                )}
            </div>
        </VerifyComponent>
    );
}

export default Ventas;
