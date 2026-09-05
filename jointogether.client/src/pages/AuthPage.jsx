// import { useState } from 'react';
// import { useAuth } from '../context/AuthContext';
// import { CityMapIllustration } from '../components/CityMapIllustration';
// import { FormField } from '../components/FormField';
// import { ApiError } from '../api/httpClient';

// const TABS = {
//   LOGIN: 'login',
//     REGISTER: 'register',
// };

// export function AuthPage() {
//   const [activeTab, setActiveTab] = useState(TABS.LOGIN);

//   return (
//     <div className="auth-page">
//       <section className="auth-info">
//         <div className="auth-info__brand">
//           <span className="auth-info__brand-dot"/>
//           <span>Aktivitetskartan</span>
//         </div>

//         {/* <div className="auth-info__text"> */}
//         <div className="auth-info__copy">
//           <h1>Hitta folk att göra saker 
//             <br/>
//             med, i din stad.
//           </h1>
//           <p>
//             Lära nya kunskaper om staden, lägg upp en aktivitet, 
//             välj hur många som får vara med och vem den är öppen för. 
//             Den dyker upp som en pinpoint på kartan direkt.
//           </p>
//         </div>
//         <CityMapIllustration />
//       </section>


//       <section className="auth-panel">
//         <div className="auth-card">
//           <div className="auth-tabs" role="tablist" aria-label="Authentication">
//             <button 
//               type='button'
//               role='tab'
//               aria-selected={activeTab === TABS.LOGIN}
//               // className={`auth-tab__tab ${activeTab === TABS.LOGIN ? 'is-active' : ''}`}
//               className={`auth-tab__tab ${activeTab === TABS.REGISTER ? 'is-active' : ''}`}

//               onClick={() => setActiveTab(TABS.LOGIN)}
//             >
//               Logga in
//             </button>
//             <button 
//               type='button'
//               role='tab'
//               aria-selected={activeTab === TABS.REGISTER}
//               className={`auth-tab__tab ${activeTab === TABS.REGISTER ? 'is-active' : ''}`}
//               onClick={() => setActiveTab(TABS.REGISTER)}
//             >
//               Skapa konto
//             </button>
//           </div>

//           {activeTab === TABS.LOGIN ? (
//             <LoginForm onSwitchToRegister={() => setActiveTab(TABS.REGISTER)} />
//           ) : (
//             <RegisterForm onSwitchToLogin={() => setActiveTab(TABS.LOGIN)} />
//           )}
//         </div>
//       </section>

//     </div>
//   )
// }

// function LoginForm() {
//   const { login } = useAuth();
//   const [form, setForm] = useState({ email: '', password: '' });
//   const [error, setError] = useState({});
//   const [formError, setFormError] = useState('');
//   const [isSubmitting, setIsSubmitting] = useState(false);

//   const updateField = (field) => (event) => 
//     setForm((prev) => ({ ...prev, [field]: event.target.value }));

//   const validate = () => {
//     const nextErrors = {};
//     if (!form.email.trim()) nextErrors.email = 'E-post måste anges';
//     if (!form.password) nextErrors.password = 'Lösenord måste anges';
//     setError(nextErrors);
//     return Object.keys(nextErrors).length === 0;
//   }

//   const handleSubmit = async (event) => {
//     event.preventDefault();
//     setFormError('');
//     if (!validate()) return;

//     setIsSubmitting(true);
//     try {
//       await login(form);
//     } catch (error) {
//       setFormError(error instanceof ApiError ? error.message : 'Felaktiga inloggningsuppgifter');
//     } finally {
//       setIsSubmitting(false);
//     }
//   }

//   return (
//     <form className="auth-form" onSubmit={handleSubmit} noValidate>
//       <h2>Välkommen</h2>
//       <p>Logga in för att se aktiviteter nära dig.</p>
//       {formError && <div className="auth-card__form-error">{formError}</div>}
      
//       <FormField 
//         id="login-email"
//         label="E-post"
//         type="email"
//         placeholder="e-post@exempel.com"
//         value={form.email}
//         onChange={updateField('email')}
//         error={error.email}
//         required
//       />
//       <FormField 
//         id="login-password"
//         label="Lösenord"
//         type="password"
//         placeholder="Lösenord"
//         value={form.password}
//         onChange={updateField('password')}
//         error={error.password}
//         required
//       />
//       <div className="auth-card__meta-row">
//         {/* <a href="/forgot-password" className="auth-form__link"> */}
//         <a href="/forgot-password" className="auth-card__link">
//         Glömt lösenord?</a>
//       </div>

//       <button type="submit" className="auth-card__submit" disabled={isSubmitting}>
//         {isSubmitting ? 'Loggar in...' : 'Logga in'}
//       </button>
      
//     </form>
//   )
// }

// function RegisterForm({onSwitchToLogin}) {
//   const { register } = useAuth();
//   const [form, setForm] = useState({ fullName: '', email: '', password: ''});
//   const [error, setError] = useState({});
//   const [formError, setFormError] = useState('');
//   const [isSubmitting, setIsSubmitting] = useState(false);

//   const updateField = (field) => (event) => 
//     setForm((prev) => ({ ...prev, [field]: event.target.value }));

//   const validate = () => {
//     const nextErrors = {};
//     if (!form.fullName.trim()) nextErrors.fullName = 'Fullständigt namn måste anges';
//     if (!form.email.trim()) nextErrors.email = 'E-post måste anges';
//     if (!form.password) {
//       nextErrors.password = 'Lösenord måste anges';
//     } else if (form.password.length < 6) {
//       nextErrors.password = 'Lösenordet måste vara minst 6 tecken';
//     }
//     setError(nextErrors);
//     return Object.keys(nextErrors).length === 0;
//   }

//   const handleSubmit = async (event) => {
//     event.preventDefault();
//     setFormError('');
//     if (!validate()) return;

//     setIsSubmitting(true);
//     try {
//       await register(form);
//     } catch (error) {
//       setFormError(error instanceof ApiError ? error.message : 'Kunde inte skapa konto');
//     } finally {
//       setIsSubmitting(false);
//     }
//   }

//   return ( 
//     <form onSubmit={handleSubmit} noValidate>
//       <h2>Skapa konto</h2>
//       <p>Det tar en minut.</p>
//       {formError && <div className="auth-card__form-error">{formError}</div>}
      
//       <FormField
//         id="register-fullName"
//         label="Namn"
//         type="text"
//         placeholder="Första och efternamn"
//         value={form.fullName}
//         onChange={updateField('fullName')}
//         error={error.fullName}
//         required 
//       />
        
//       <FormField
//         id="register-email"
//         label="E-post"
//         type="email"
//         placeholder="e-post@exempel.com"
//         value={form.email}
//         onChange={updateField('email')}
//         error={error.email}
//         required 
//       />
      
//       <FormField
//         id="register-password"
//         label="Lösenord"
//         type="password"
//         placeholder="Lösenord"
//         value={form.password}
//         onChange={updateField('password')}
//         error={error.password}
//         required 
//       />

//       <button type="submit" className="auth-card__submit" disabled={isSubmitting}>
//         {/* {isSubmitting ? 'Skapar konto...' : 'Skapa konto'} */}
//         {isSubmitting ? 'Fortsätter...' : 'Fortsätt'}
//       </button>

//       {/* <p className="auth-card__footnote">
//         Already have an account?{' '}
//         <button type="button" className="auth-card__link auth-card__link--inline" onClick={onSwitchToLogin}>
//           Log in
//         </button>
//       </p> */}

//       <p className="auth-card__footnote">
//           Har du redan ett konto?{' '}
//         <button type="button" className="auth-card__link auth-card__link--inline" onClick={onSwitchToLogin}>
//         Logga in
//        </button>
//       </p>
//     </form>
//   );
// }

import { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { CityMapIllustration } from '../components/CityMapIllustration';
import { FormField } from '../components/FormField';
import { ApiError } from '../api/httpClient';

const TABS = {
  LOGIN: 'login',
  REGISTER: 'register',
};

export function AuthPage() {
  const [activeTab, setActiveTab] = useState(TABS.LOGIN);

  return (
    <div className="auth-page">
      <section className="auth-info">
        <div className="auth-info__brand">
          <span className="auth-info__brand-dot" />
          <span>Aktivitetskartan</span>
        </div>

        <div className="auth-info__copy">
          <h1>Hitta folk att göra saker
            <br />
            med, i din stad.
          </h1>
          <p>
            Lära nya kunskaper om staden, lägg upp en aktivitet,
            välj hur många som får vara med och vem den är öppen för.
            Den dyker upp som en pinpoint på kartan direkt.
          </p>
        </div>
        <CityMapIllustration />
      </section>

      <section className="auth-panel">
        <div className="auth-card">
          <div className="auth-tabs" role="tablist" aria-label="Authentication">
            <button
              type="button"
              role="tab"
              aria-selected={activeTab === TABS.LOGIN}
              className={`auth-tabs__tab ${activeTab === TABS.LOGIN ? 'is-active' : ''}`}
              onClick={() => setActiveTab(TABS.LOGIN)}
            >
              Logga in
            </button>
            <button
              type="button"
              role="tab"
              aria-selected={activeTab === TABS.REGISTER}
              className={`auth-tabs__tab ${activeTab === TABS.REGISTER ? 'is-active' : ''}`}
              onClick={() => setActiveTab(TABS.REGISTER)}
            >
              Skapa konto
            </button>
          </div>

          {activeTab === TABS.LOGIN ? (
            <LoginForm onSwitchToRegister={() => setActiveTab(TABS.REGISTER)} />
          ) : (
            <RegisterForm onSwitchToLogin={() => setActiveTab(TABS.LOGIN)} />
          )}
        </div>
      </section>
    </div>
  );
}

function LoginForm() {
  const { login } = useAuth();
  const [form, setForm] = useState({ email: '', password: '' });
  const [error, setError] = useState({});
  const [formError, setFormError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const updateField = (field) => (event) =>
    setForm((prev) => ({ ...prev, [field]: event.target.value }));

  const validate = () => {
    const nextErrors = {};
    if (!form.email.trim()) nextErrors.email = 'E-post måste anges';
    if (!form.password) nextErrors.password = 'Lösenord måste anges';
    setError(nextErrors);
    return Object.keys(nextErrors).length === 0;
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setFormError('');
    if (!validate()) return;

    setIsSubmitting(true);
    try {
      await login(form);
    } catch (error) {
      setFormError(error instanceof ApiError ? error.message : 'Felaktiga inloggningsuppgifter');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form className="auth-form" onSubmit={handleSubmit} noValidate>
      <h2>Välkommen</h2>
      <p>Logga in för att se aktiviteter nära dig.</p>
      {formError && <div className="auth-card__form-error">{formError}</div>}

      <FormField
        id="login-email"
        label="E-post"
        type="email"
        placeholder="e-post@exempel.com"
        value={form.email}
        onChange={updateField('email')}
        error={error.email}
        required
      />
      <FormField
        id="login-password"
        label="Lösenord"
        type="password"
        placeholder="Lösenord"
        value={form.password}
        onChange={updateField('password')}
        error={error.password}
        required
      />
      <div className="auth-card__meta-row">
        <a href="/forgot-password" className="auth-card__link">
          Glömt lösenord?
        </a>
      </div>

      <button type="submit" className="auth-card__submit" disabled={isSubmitting}>
        {isSubmitting ? 'Loggar in...' : 'Logga in'}
      </button>
    </form>
  );
}

function RegisterForm({ onSwitchToLogin }) {
  const { register } = useAuth();
  const [form, setForm] = useState({ fullName: '', email: '', password: '' });
  const [error, setError] = useState({});
  const [formError, setFormError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const updateField = (field) => (event) =>
    setForm((prev) => ({ ...prev, [field]: event.target.value }));

  const validate = () => {
    const nextErrors = {};
    if (!form.fullName.trim()) nextErrors.fullName = 'Fullständigt namn måste anges';
    if (!form.email.trim()) nextErrors.email = 'E-post måste anges';
    if (!form.password) {
      nextErrors.password = 'Lösenord måste anges';
    } else if (form.password.length < 6) {
      nextErrors.password = 'Lösenordet måste vara minst 6 tecken';
    }
    setError(nextErrors);
    return Object.keys(nextErrors).length === 0;
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setFormError('');
    if (!validate()) return;

    setIsSubmitting(true);
    try {
      await register(form);
    } catch (error) {
      setFormError(error instanceof ApiError ? error.message : 'Kunde inte skapa konto');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} noValidate>
      <h2>Skapa konto</h2>
      <p>Det tar en minut.</p>
      {formError && <div className="auth-card__form-error">{formError}</div>}

      <FormField
        id="register-fullName"
        label="Namn"
        type="text"
        placeholder="Första och efternamn"
        value={form.fullName}
        onChange={updateField('fullName')}
        error={error.fullName}
        required
      />

      <FormField
        id="register-email"
        label="E-post"
        type="email"
        placeholder="e-post@exempel.com"
        value={form.email}
        onChange={updateField('email')}
        error={error.email}
        required
      />

      <FormField
        id="register-password"
        label="Lösenord"
        type="password"
        placeholder="Lösenord"
        value={form.password}
        onChange={updateField('password')}
        error={error.password}
        required
      />

      <button type="submit" className="auth-card__submit" disabled={isSubmitting}>
        {isSubmitting ? 'Fortsätter...' : 'Fortsätt'}
      </button>

      <p className="auth-card__footnote">
        Har du redan ett konto?{' '}
        <button type="button" className="auth-card__link auth-card__link--inline" onClick={onSwitchToLogin}>
          Logga in
        </button>
      </p>
    </form>
  );
}