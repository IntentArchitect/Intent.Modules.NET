import { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "./useAuth";

export function RequireAuth({ roles, children }: { roles?: string[]; children: ReactNode }) {
  const { user } = useAuth();
  if (!user || (roles && !roles.some((role) => user.roles.includes(role)))) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
}
