'use client';
import { useEffect, useState } from 'react';
import Link from 'next/link';
import { Menu, X, ShoppingCart } from 'react-feather';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Navbar, Nav } from 'react-bootstrap';
//PROYECTO12
interface CustomAccordionItemProps {
  title: string;
  href?: string;
  icon?: React.ReactNode;
  onClick?: () => void;
}

const CustomAccordionItem: React.FC<CustomAccordionItemProps> = ({ title, href, icon, onClick }) => {
  const [isOpen, setIsOpen] = useState(false);

  const toggleAccordion = () => {
    setIsOpen(!isOpen);
  };

  return (
    <div className="my-4">
      <div
        className="d-flex justify-content-between align-items-center bg-light p-3"
        style={{ cursor: 'pointer' }}
        onClick={onClick || toggleAccordion}
      >
        <h3 className="mb-0">{title}</h3>
        {isOpen ? <X /> : <Menu />}
      </div>
      {isOpen && (
        <div className="p-3">
          <div className="mb-3">
            <Link href={href || '#'}>
              <button className="btn btn-secondary btn-sm btn-block">
                Ir a Ventas
              </button>
            </Link>
          </div>
          <div className="mb-3">
            <Link href="/products">
              <button className="btn btn-secondary btn-sm btn-block">
                Ir a Productos
              </button>
            </Link>
          </div>
          <div className="mb-3">
            <Link href="/campannas">
              <button className="btn btn-secondary btn-sm btn-block">
                Ir a Campañas
              </button>
            </Link>
          </div>
          <div className="mb-3">
            <Link href="/payment">
              <button className="btn btn-secondary btn-sm btn-block">
                Ir a Pagos
              </button>
            </Link>
          </div>
        </div>
      )}
    </div>
  );
};

const Init: React.FC = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [hasToken, setHasToken] = useState(true);

  useEffect(() => {
    const token = sessionStorage.getItem('authToken');
    if (!token) {
      setHasToken(false);
      window.location.href = '/admin';
    }
  }, []);

  const toggleMenu = () => {
    setIsOpen(!isOpen);
  };

  if (!hasToken) {
    return null; // Render nothing if there's no token
  }

  return (
    <div className="container-fluid">
      <Navbar bg="light" expand="lg">
        <Navbar.Toggle onClick={toggleMenu} aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav" className={isOpen ? 'show' : ''}>
          <Nav className="flex-column mt-4">
            <CustomAccordionItem
              title="Panel Administración"
              href="/ventas"
              icon={<ShoppingCart />}
            />
          </Nav>
        </Navbar.Collapse>
      </Navbar>
    </div>
  );
};

export default Init;
