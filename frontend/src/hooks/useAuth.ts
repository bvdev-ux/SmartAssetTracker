"use client";

import { useEffect, useState } from "react";
import type { User } from "@/types";

export function useAuth() {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const stored = localStorage.getItem("user");
    setUser(stored ? (JSON.parse(stored) as User) : null);
    setIsLoading(false);
  }, []);

  const logout = () => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("user");
    setUser(null);
  };

  return { user, isLoading, logout, isAuthenticated: !!user };
}
