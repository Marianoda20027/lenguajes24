import { useEffect, useState } from "react";
import Alert from 'react-bootstrap/Alert';

const PaymentForm = ({ cart, setCart, clearProducts }:
    { cart: any, setCart: (cart: any) => void, clearProducts: () => void }) => {

    const [isMessageShowing, setIsMessageShowing] = useState(false);
    const [message, setMessage] = useState('');
    const [alertType, setAlertType] = useState(0);
    const [orderNumber, setOrderNumber] = useState('');
    const [selectedIndex, setSelectedIndex] = useState(0);
    const [finishedSale, setFinishedSale] = useState(false);

    enum PaymentMethod {
        EFECTIVO = 0,
        SINPE = 1
    }

    useEffect(() => {
        setCart(cart => ({
            ...cart,
            carrito: {
                ...cart.carrito,
                metodoDePago: cart.carrito.metodoDePago ? cart.carrito.metodoDePago : PaymentMethod.EFECTIVO
            }
        }));
        setSelectedIndex((cart.carrito.metodoDePago === PaymentMethod.EFECTIVO) ? 0 : 1);
    }, []);

    function handleSelectPayment(event: any) {
        setSelectedIndex(event.target.selectedIndex);
        setCart(cart => ({
            ...cart,
            carrito: {
                ...cart.carrito,
                metodoDePago: event.target.selectedIndex === 0 ? PaymentMethod.EFECTIVO : PaymentMethod.SINPE
            },
            necesitaVerificacion: true
        }));
    }

    function generateReceiptNumber() {
        const timestamp = Date.now().toString();
        const randomNumber = Math.floor(Math.random() * 10000).toString().padStart(4, '0');
        setOrderNumber(timestamp + randomNumber);
    }

    async function persistPurchase() {

        let purchaseToPersist = {
            "productIds": cart.carrito.productos.map(product => product.uuid),
            "address": cart.carrito.direccionEntrega,
            "paymentMethod": cart.carrito.metodoDePago
        }

        try {
            const res = await fetch('https://localhost:7151/api/Cart', {
                method: 'POST',
                body: JSON.stringify(purchaseToPersist),
                headers: {
                    'content-type': 'application/json'
                }
            })
            if (res.ok) { setMessage("Se realizó su compra"); setAlertType(0) }
            else { setMessage("Error al realizar la compra"); setAlertType(1) }
        } catch (error) {
            setMessage(error);
            setAlertType(1)
        } finally {
            setIsMessageShowing(true);
        }
    }

    async function finishPurchase() {
        generateReceiptNumber();
        setFinishedSale(true);
        clearProducts();
        await persistPurchase();
    }

    const Efectivo = () => {
        return <div className="card w-100">
            <div className="card-body">
                <div className="d-grid w-100 justify-content-center">
                    <label>Número de compra: {orderNumber}</label>
                    <label>Por favor espere la confirmación de pago por parte del administrador</label>
                </div>
            </div>
        </div>
    }

    const Sinpe = () => {
        return <div className="card w-100">
            <div className="card-body">
                <div className="d-grid w-100 justify-content-center">
                    <label>Número de compra: {orderNumber}</label>
                    <label>Número para realizar el pago: +506 8888 8888</label>
                    <label>Por favor espere la confirmación de pago por parte del administrador</label>
                </div>
            </div>
        </div>
    }

    return <div className="container">
        <div className="d-grid justify-content-center gap-4">
            <div className="container">
                <h3>Métodos de pago</h3>
                <div className="form-group">
                    <select className="form-control" onChange={handleSelectPayment}
                        value={cart.carrito.metodoDePago} disabled={finishedSale}>
                        {cart.metodosDePago.map((method: any, index: number) => <option key={index}>{method}</option>)}
                    </select>
                </div>
                <div className="d-flex w-100 justify-content-center">
                    <button className="btn btn-primary"
                        disabled={finishedSale} onClick={finishPurchase}>
                        Finalizar Compra</button>
                </div>
            </div>
            {finishedSale ? (selectedIndex === 0 ? <Efectivo /> : <Sinpe />) : ''}
            {finishedSale ? <div className="progress">
                <div className="progress-bar progress-bar-striped progress-bar-animated"
                    role="progressbar" aria-valuenow={75} aria-valuemin={0}
                    aria-valuemax={100} style={{ width: '100%' }}></div>
            </div> : ''}
            {isMessageShowing ?
                <div
                    style={{
                        position: 'fixed',
                        bottom: 20,
                        right: 20,
                        zIndex: 9999, // Ensure it's above other content
                    }}
                >
                    <Alert variant={alertType === 0 ? "success" : "danger"}
                        onClose={() => setIsMessageShowing(false)} dismissible>
                        <Alert.Heading>{alertType === 0 ? 'Información' : 'Error'}</Alert.Heading>
                        <p>{message.toString()}</p>
                    </Alert> </div> : ''
            }
        </div>
    </div>
}

export default PaymentForm;