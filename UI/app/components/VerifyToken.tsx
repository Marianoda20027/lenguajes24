// components/VerifyToken.tsx
"use client";
import React, { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { jwtDecode } from 'jwt-decode';


export const verifyToken = (router) => {
    
    const loginToken = sessionStorage.getItem("authToken");

    if (!loginToken) {
        return false;
    }

    const tokenFormat = jwtDecode(loginToken);
    const todayDate = Date.now() / 1000;

    if (tokenFormat.exp && tokenFormat.exp < todayDate) {
        router.push("/../admin");
        sessionStorage.removeItem("authToken");
        return false;
    }

    return true;
};

export default function VerifyComponent({ children }: { children: React.ReactNode }) {
    const router = useRouter();

    useEffect(() => {
        if (!verifyToken(router)) {
            router.push("/../admin");
        }
    }, [router]);

    return <>{children}</>;
}
