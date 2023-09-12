using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using UnityEngine;
namespace Hiyazcool
{
	/*
	 * Todo 
	 *	-Better Organize Statistics
	 *	-General Statistics ie score, playtime on save, etc
	 *	-Finish this
	 */
	public static class Statistics
	{
		public static class Damage
		{
			public delegate void HitContextCallback(HitContext hitContext);
			public interface IDamagable
			{
				public abstract void OnDamage(DamageContext context, HitContextCallback callback = null); // Delegate referencing Function that recieved damage done
			}
			public struct HitContext
			{
				public int damageDone;
			}
			public struct DamageContext
			{
				public int damage;
				public int damageArmorPiercing;
				public DamageType type;
			}
			public enum DamageType
			{
				none,
				acid,
				cold,
				dark,
				fire,
				lightning,
				necrotic,
				poison,
				radiant,
			}
			public struct DamageStats
			{
				public int damage;
				public int damageArmorPiercing;
				public DamageType type;
				public int attackSpeed;
			}
		}
		public static class Effect
		{
			public delegate void EffectContextCallback(EffectContext effectContext);
			public interface IEffectable
			{
				public abstract ref readonly HitEffectContext OnEffect(EffectContext context, EffectContextCallback callback);
			}
			public struct EffectContext
			{

			}
			public struct HitEffectContext
			{

			}

		}
		public static class Health
		{
			public struct HealthStats
			{
				public int health
				{ get; private set; }
				public int maxHealth
				{ get; private set; }
				public int overHealth
				{ get; private set; }
				public int barrierHealth
				{ get; private set; }
				public int maxBarrierHealth
				{ get; private set; }
				public int overBarrierHealth
				{ get; private set; }
				public int TakeHealth(int amount, bool isBarrierPiercing = false)
				{
					int remaining;
					if (isBarrierPiercing)
					{
						overHealth -= amount;

						if (overHealth < 0)
						{
							remaining = -overHealth;
							health -= remaining;
							if (health <= 0)
								amount += health;
						}
					}
					else
					{
						overBarrierHealth -= amount;
						if (overBarrierHealth < 0)
						{
							remaining = -overBarrierHealth;
							barrierHealth -= remaining;
							if (barrierHealth <= 0)
							{
								amount = -barrierHealth;
								overHealth -= remaining;

								if (overHealth < 0)
								{
									remaining = -overHealth;
									health -= remaining;
									if (health <= 0)
										amount += health;
								}
								return amount;
							}
							else
								amount = 0;
						}
						else
							amount = 0;
					}
					CheckIfAtMaxStats();
					return amount;
				}
				public int GiveHealth(int amount, bool isOverHealth = false)
				{
					return 0;
				}
				public int GiveBarrierHealth(int amount, bool isOverBarrierHealth = false)
				{
					return 0;
				}
				public bool CheckIfAlive()
				{
					return health > 0;
				}
				public bool CheckIfAtMaxStats()
				{
					return true;
				}

			}
		}
	}
}