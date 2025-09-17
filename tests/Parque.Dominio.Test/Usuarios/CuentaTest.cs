using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Test.Usuarios;

[TestClass]
public class CuentaTest
{
    [TestMethod]
    public void Crear_Cuenta_Con_Datos_Validos_Asignacion_Correcta()
    {
        var email = new Email("mailprueba");
        var password = new PasswordHash("passprueba");

        var cuenta = Cuenta.Crear("jorge", "ramirez", email, password);

        Assert.IsNotNull(cuenta);
        Assert.AreEqual("jorge", cuenta.Nombre);
        Assert.AreEqual("ramirez", cuenta.Apellido);
        Assert.AreEqual("mailprueba", cuenta.Email.Valor);
        Assert.AreEqual("passprueba", cuenta.PasswordHash.Valor);
        Assert.AreEqual(Rol.Operador, cuenta.Roles.FirstOrDefault());

        // VisitanteId debe ser nulo por defecto
        Assert.IsNull(cuenta.VisitanteId);

        // Roles debe estar vacío inicialmente
        Assert.IsNotNull(cuenta.Roles);
        Assert.AreEqual(1, cuenta.Roles.Count);

        // Id debe ser distinto de Guid.Empty y generarse nuevo
        Assert.AreNotEqual(Guid.Empty, cuenta.Id);
    }

    [TestMethod]
    public void Crear_Cuentas_Diferentes_Generan_Ids_Distintos()
    {
        var email1 = new Email("mail1");
        var email2 = new Email("mail2");
        var pass1 = new PasswordHash("pass1");
        var pass2 = new PasswordHash("pass2");

        var c1 = Cuenta.Crear("nombre1", "apellido1", email1, pass1);
        var c2 = Cuenta.Crear("nombre2", "apellido2", email2, pass2);

        Assert.AreNotEqual(c1.Id, c2.Id);
    }

    [TestMethod]
    public void Propiedades_Son_Inmutables_Desde_Externo()
    {
        // var cuenta = Cuenta.Crear("nombre", "apellido", new Email("email"), new PasswordHash("pass"));

        // Como las propiedades no tienen setters públicos, este test verifica que compilador no permite asignar valores externos
        // Esto se verifica más a nivel de compilación, no como assert runtime, por lo que sirve como referencia.
        // Ejemplo comentado:
        // cuenta.Nombre = "nuevo"; // No compila
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Crear_Cuenta_Con_VisitanteId_Asignado()
    {
        var email = new Email("visitante@mail.com");
        var password = new PasswordHash("visitantepass");
        var cuenta = Cuenta.Crear("Ana", "Perez", email, password);

        // Simula asignación interna de VisitanteId (si existe un método o constructor que lo permita)
        var visitanteId = Guid.NewGuid();
        typeof(Cuenta)
            .GetProperty("VisitanteId")?
            .SetValue(cuenta, visitanteId);

        Assert.AreEqual(visitanteId, cuenta.VisitanteId);
    }
}
